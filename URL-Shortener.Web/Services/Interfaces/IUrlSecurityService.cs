namespace URL_Shortener.Web.Services.Interfaces
{
    public interface IUrlSecurityService
    {
        bool IsValidUri(string url);
        Task<bool> IsUrlSafeAsync(string url);
    }
}