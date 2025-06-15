using System.Xml;
using URL_Shortener.Web.Data.Entities;
using URL_Shortener.Web.Data.Entities.Enums;
using URL_Shortener.Web.Models.ClickDetails;

namespace URL_Shortener.Web.Models.UrlVMs
{
    public record UrlReadOnlyVM
    {
        public UrlReadOnlyVM(string targetUrl, AnalitycsType analitycs, DateTime expiresAt)
        {
            ShortenedUrl = string.Empty;
            TargetUrl = targetUrl;
            Analitycs = analitycs;
            ExpiresAt = expiresAt;
            ClickDetails = [];
        }

        public UrlReadOnlyVM(string shortenedUrl, string targetUrl, AnalitycsType analitycs, IEnumerable<ClickDetailReadOnlyVM> clickDetails)
        {
            ShortenedUrl = shortenedUrl;
            TargetUrl = targetUrl;
            Analitycs = analitycs;
            ClickDetails = (List<ClickDetailReadOnlyVM>)clickDetails;
        }

        public UrlReadOnlyVM(Url url, Uri uri)
        {
            Id = url.Id;
            ShortenedUrl = uri.ToString() + "r/" + url.ShortenedURL;
            TargetUrl = url.TargetURL;
            Analitycs = url.AnalitycsMode;
            ShortenedAt = url.CreatedAt;
            ExpiresAt = url.ExpiresAt;

            ClickDetails = [];
            if (url.ClickDetails is not null && url.ClickDetails.Count > 0)
            {
                foreach (var click in url.ClickDetails)
                {
                    ClickDetails.Add(new ClickDetailReadOnlyVM(click.Timestamp, click.IPAdress, click.UserAgent));
                }
            }
        }

        public long Id { get; }
        public string ShortenedUrl { get; }
        public string TargetUrl { get; }
        public AnalitycsType Analitycs { get; }
        public DateTime ShortenedAt { get; }
        public DateTime ExpiresAt { get; }

        public List<ClickDetailReadOnlyVM> ClickDetails { get; }
    }
}
