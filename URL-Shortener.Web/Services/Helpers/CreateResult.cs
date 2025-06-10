namespace URL_Shortener.Web.Services.Helpers
{
    public sealed record CreateResult
    {
        public CreateResult(string shortUrl, string targetUrl)
        {
            ShortUrl = shortUrl;
            TargetUrl = targetUrl;
        }

        public string ShortUrl { get; }
        public string TargetUrl {  get; }
    }
}
