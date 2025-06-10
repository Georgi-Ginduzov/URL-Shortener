using URL_Shortener.Web.Services.Helpers;

namespace URL_Shortener.Web.Services.Interfaces
{
    public interface IShorteningService
    {
        Task<CreateResult> ShortenUrl(string targetUrl, HttpContext httpContext);
        Task<RedirectResult> GetRedirectionUrl(string shortUrl, HttpContext httpContext);
    }
}