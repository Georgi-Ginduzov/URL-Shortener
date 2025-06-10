namespace URL_Shortener.Web.Services.Helpers
{
    public sealed record RedirectResult
    {
        public RedirectResult(RedirectStatus status, string? targetUrl = null)
        {
            Status = status;
            TargetUrl = targetUrl;
        }

        public RedirectStatus Status { get; }
        public string? TargetUrl { get; }
    }
}
