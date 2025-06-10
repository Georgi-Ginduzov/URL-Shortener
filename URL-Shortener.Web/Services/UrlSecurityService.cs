using URL_Shortener.Web.Services.Interfaces;

namespace URL_Shortener.Web.Services
{
    public class UrlSecurityService : IUrlSecurityService
    {

        public async ValueTask<bool> IsUrlSecure(string url)
        {
            // Is url valid
            try
            {
                var isValidUri = Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);

                if (!url.StartsWith("https"))
                    return false;

                //...
            }
            catch
            {

                return false;
            }

            return true;
        }
    }
}
