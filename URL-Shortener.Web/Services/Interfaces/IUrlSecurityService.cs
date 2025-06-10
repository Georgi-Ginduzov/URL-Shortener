namespace URL_Shortener.Web.Services.Interfaces
{
    public interface IUrlSecurityService
    {
        ValueTask<bool> IsUrlSecure(string url);
    }
}