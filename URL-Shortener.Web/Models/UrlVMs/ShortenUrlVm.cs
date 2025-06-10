using System.ComponentModel.DataAnnotations;

namespace URL_Shortener.Web.Models.UrlVMs
{
    public class ShortenUrlVm
    {
        [Required, Url, Display(Name = "URL to shorten")]
        public string TargetUrl { get; set; } = string.Empty;
        
        public string? ShortUrl { get; set; }
        public string? Error { get; set; }
    }
}
