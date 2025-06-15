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
        private readonly IUnitOfWork worker;
        private readonly IUrlSecurityService securityService;

        public ShorteningService(IUnitOfWork unitOfWork, IUrlSecurityService securityService)
        {
            this.worker = unitOfWork;
            this.securityService = securityService;
        }

        public async Task<CreateResult> ShortenUrl(string targetUrl, HttpContext httpContext)
        {
            var sessionId = httpContext.Session.GetOrCreateGuid();
            await worker.SessionRepository.GetOrCreateAsync(sessionId);

            if (!securityService.IsValidUri(targetUrl))
                return new CreateResult(null, null, "The provided uri for shortening is not a valid uri");

            var uriSecurityCheck = securityService.IsUrlSafeAsync(targetUrl);
            var shortUrl = NewShortenedUrl(targetUrl, httpContext);

            try
            {
                await Task.WhenAll(uriSecurityCheck, shortUrl);

                if (!uriSecurityCheck.Result)
                {
                    worker.UrlRepository.Remove(shortUrl.Result);
                    await worker.SaveAsync();

                    return new CreateResult(null, null, "The provided url is harmful hence it won't be shortened!");
                }

                await worker.SaveAsync();

                return new CreateResult(shortUrl.Result.ShortenedURL, shortUrl.Result.TargetURL);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while shortening the url: {targetUrl}.";
                Console.WriteLine(errorMessage + $"\\nException message:\\n{ex.Message}StackTrace:\\n{ex.StackTrace}");

                if (!shortUrl.IsCompleted)
                {
                    worker.UrlRepository.Remove(shortUrl.Result);
                    await worker.SaveAsync();
                }

                return new CreateResult(null, null, errorMessage);
            }
        }

        public async Task<RedirectResult> GetRedirectionUrl(string shortUrl, HttpContext httpContext)
        {
            var sessionId = httpContext.Session.GetOrCreateGuid();
            await worker.SessionRepository.GetOrCreateAsync(sessionId);
            await worker.SessionRepository.TouchAsync(sessionId);

            if (!Cryptography.IsValidBase62Slug(shortUrl))
                return new RedirectResult(RedirectStatus.NotFound);

            var convertedUrl = Cryptography.Base62ConvertToDecimal(shortUrl);

            if (!IsLoggedInUser(httpContext))
            {
                var urlRecord = await worker.UrlRepository.GetByIdAsync(convertedUrl);
                if (urlRecord != null)
                    return new RedirectResult(RedirectStatus.Found, urlRecord.TargetURL);
                else
                    return new RedirectResult(RedirectStatus.NotFound);
            }

            // Based on the analytics do the associated thing
            var userId = httpContext.User.Identity.GetUserId();
            var url = await worker.UrlRepository.GetByIdAndUserIdAsync(convertedUrl, userId);

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
                            await worker.ClickDetailsRepository.AddAsync(url.Id); 
                        }
                        catch (Exception ex) 
                        { Console.WriteLine(ex.Message + "Click logging failed"); }
                    });
                    break;
                case AnalitycsType.SaveLocation:
                case AnalitycsType.SaveClicksAndLocation:
                    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await worker.ClickDetailsRepository.AddAsync(url.Id, ipAddress);
                        }
                        catch (Exception ex)
                        { 
                            Console.WriteLine(ex.Message + "Click logging failed"); 
                        }
                    });
                    break;
                default:
                    return new RedirectResult(RedirectStatus.Error);
            }
            
            return new RedirectResult(RedirectStatus.Found, url.TargetURL);
        }

        private bool IsLoggedInUser(HttpContext httpContext) => httpContext.User.Identity?.IsAuthenticated == true;

        private async Task<Url> NewShortenedUrl(string targetUrl, HttpContext httpContext)
        {
            Url shortUrl;
            if (!IsLoggedInUser(httpContext))
            {
                shortUrl = await worker.UrlRepository
                    .AddNewUrlAsync(targetUrl, Guid.Parse(httpContext.Session.Id));
            }
            else
            {
                var userId = httpContext.User.Identity.GetUserId();
                shortUrl = await worker.UrlRepository
                    .AddNewUrlAsync(targetUrl, userId, Guid.Parse(httpContext.Session.Id));
            }

            await worker.SaveAsync();
            return shortUrl;
        }
    }
}
