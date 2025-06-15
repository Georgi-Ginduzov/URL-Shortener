using System.ComponentModel.DataAnnotations;
using URL_Shortener.Web.Data.Entities.Enums;

namespace URL_Shortener.Web.Models.UrlVMs
{
    public record ShortenUrlVm
    {
        public ShortenUrlVm()
        {
            TargetUrl = string.Empty;
            AnalyticsMode = AnalitycsType.None;
            ShortUrl = string.Empty;
            Error = string.Empty;
        }

        public ShortenUrlVm(string? targetUrl, AnalitycsType? analitycs, string? shortUrl, string? error)
        {
            TargetUrl = targetUrl ?? string.Empty;
            AnalyticsMode = analitycs ?? AnalitycsType.None;
            ShortUrl = shortUrl;
            Error = error;
        }

        [Required(ErrorMessage = "Target url is required")]
        [Display(Name = "URL to shorten")]
        public string TargetUrl { get; set; }
        public AnalitycsType AnalyticsMode { get; }
        public string? ShortUrl { get; set; }
        public string? Error { get; set; }
    }
}
