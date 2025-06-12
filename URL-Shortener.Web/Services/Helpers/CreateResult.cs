namespace URL_Shortener.Web.Services.Helpers
{
    public sealed record CreateResult
    {
        public CreateResult(string? shortUrl, string? targetUrl)
        {
            ShortUrl = shortUrl;
            TargetUrl = targetUrl;
        }

        public CreateResult(string? shortUrl, string? targetUrl, string errorMessage) : this(shortUrl, targetUrl)
        {
            ErrorMessage = errorMessage;
        }

        public string? ShortUrl { get; }
        public string? TargetUrl {  get; }
        public string? ErrorMessage { get; }
    }
}
