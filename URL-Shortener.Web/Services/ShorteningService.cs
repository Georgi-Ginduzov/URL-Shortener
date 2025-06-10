using Microsoft.AspNet.Identity;
using URL_Shortener.Web.Data.Entities;
using URL_Shortener.Web.Data.Entities.Enums;
using URL_Shortener.Web.Repositories.Interfaces;
using URL_Shortener.Web.Services.Helpers;
using URL_Shortener.Web.Services.Interfaces;
using URL_Shortener.Web.Utilities;

namespace URL_Shortener.Web.Services
{
    public class ShorteningService : IShorteningService
    {
        private readonly IUnitOfWork unitOfWork;

        public ShorteningService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<CreateResult> ShortenUrl(string targetUrl, HttpContext httpContext)
        {
            var sessionId = httpContext.Session.GetOrCreateGuid();
            await unitOfWork.SessionRepository.GetOrCreateAsync(sessionId);

            // Security check

            Url shortUrl;
            if (!IsLoggedInUser(httpContext))
            {
                shortUrl = await unitOfWork.UrlRepository
                    .AddNewUrlAsync(targetUrl, Guid.Parse(httpContext.Session.Id));
            }
            else
            {
                var userId = httpContext.User.Identity.GetUserId();
                shortUrl = await unitOfWork.UrlRepository
                    .AddNewUrlAsync(targetUrl, userId, Guid.Parse(httpContext.Session.Id));
            }

            await unitOfWork.SaveAsync();

            return new CreateResult(shortUrl.ShortenedURL, shortUrl.TargetURL);
        }

        public async Task<RedirectResult> GetRedirectionUrl(string shortUrl, HttpContext httpContext)
        {
            var sessionId = httpContext.Session.GetOrCreateGuid();
            await unitOfWork.SessionRepository.GetOrCreateAsync(sessionId);

            if (!Cryptography.IsValidBase62Slug(shortUrl))
                return new RedirectResult(RedirectStatus.NotFound);

            var convertedUrl = Cryptography.Base62ConvertToDecimal(shortUrl);

            if (!IsLoggedInUser(httpContext))
            {
                var urlRecord = await unitOfWork.UrlRepository.GetByIdAsync(convertedUrl);
                if (urlRecord != null)
                    return new RedirectResult(RedirectStatus.Found, urlRecord.TargetURL);
                else
                    return new RedirectResult(RedirectStatus.NotFound);
            }

            // Based on the analytics do the associated thing
            var userId = httpContext.User.Identity.GetUserId();
            var url = await unitOfWork.UrlRepository.GetByIdAndUserIdAsync(convertedUrl, userId);

            if (url == null)
                return new RedirectResult(RedirectStatus.NotFound);
            if (url.ExpiresAt < DateTime.Now)
                return new RedirectResult(RedirectStatus.Expired);

            switch (url.AnalitycsMode)
            {
                case AnalitycsType.None:
                    break;
                case AnalitycsType.SaveClicks:
                    // awaiting could be optional if the client wants to have faster redirects
                    _ = Task.Run(async () =>
                    {
                        try 
                        { 
                            await unitOfWork.ClickDetailsRepository.AddAsync(url.Id); 
                        }
                        catch (Exception ex) 
                        { Console.WriteLine(ex.Message + "Click logging failed"); }
                    });

                    await unitOfWork.ClickDetailsRepository.AddAsync(url.Id);
                    break;
                case AnalitycsType.SaveLocation:
                case AnalitycsType.SaveClicksAndLocation:
                    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await unitOfWork.ClickDetailsRepository.AddAsync(url.Id, ipAddress);
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.Message + "Click logging failed"); }
                    });
                    break;
                default:
                    return new RedirectResult(RedirectStatus.Error);
            }
            
            return new RedirectResult(RedirectStatus.Found, url.TargetURL);
        }

        private bool IsLoggedInUser(HttpContext httpContext) => httpContext.User.Identity?.IsAuthenticated == true;
    }
}
