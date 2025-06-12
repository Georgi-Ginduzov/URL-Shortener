namespace URL_Shortener.Web.ExternalSystems.SafeBrowsing.Models
{
    public class SafeBrowsingResponse
    {
        public IList<ThreatMatch>? Matches { get; set; }
    }

}
