namespace URL_Shortener.Web.Models.ClickDetails
{
    public record ClickDetailReadOnlyVM
    {
        public ClickDetailReadOnlyVM(DateTime dateTime, string ipAddress, string? userAgent)
        {
            TargetUrl = string.Empty;
            ShortenedUrl = string.Empty;
            DateTime = dateTime;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }

        public string TargetUrl { get; set; }
        public string ShortenedUrl { get; set; }
        public DateTime DateTime { get; set; }
        public string IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
