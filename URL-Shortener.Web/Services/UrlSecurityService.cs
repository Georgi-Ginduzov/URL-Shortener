using Microsoft.Extensions.Options;
using URL_Shortener.Web.ExternalSystems.SafeBrowsing.Models;
using URL_Shortener.Web.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace URL_Shortener.Web.Services
{
    public class UrlSecurityService : IUrlSecurityService
    {
        private readonly HttpClient http;
        private readonly SafeBrowsingOptions opts;

        public UrlSecurityService(HttpClient http, IOptions<SafeBrowsingOptions> opts)
        {
            this.http = http;
            this.opts= opts.Value;
        }

        public bool IsValidUri(string url)
        {
            var isUriCreated = Uri.TryCreate(url, UriKind.Absolute, out var uri);

            if (isUriCreated && url.ToLower().StartsWith("https"))
            {
                return true;
            }

            return false;
        }

        public async Task<bool> IsUrlSafeAsync(string url)
        {
            var reqest = new SafeBrowsingRequest
            {
                Client = new ClientInfo
                {
                    ClientId = opts.ClientId,
                    ClientVersion = opts.ClientVersion
                },
                ThreatInfo = new ThreatInfo
                {
                    ThreatEntries = [new ThreatEntry { Url = url }]
                }
            };
            var uri = $"https://safebrowsing.googleapis.com/v4/threatMatches:find?key={opts.ApiKey}";
            var response = await http.PostAsJsonAsync(uri, reqest);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<SafeBrowsingResponse>();
            
            return body?.Matches == null || body.Matches.Count == 0;
        }
    }
}
