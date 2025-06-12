namespace URL_Shortener.Web.ExternalSystems.SafeBrowsing.Models
{
    public class ThreatMatch
    {
        public string ThreatType { get; set; } = "";
        public string PlatformType { get; set; } = "";
        public string ThreatEntryType { get; set; } = "";
        public ThreatEntry Threat { get; set; } = new ThreatEntry();
    }

}
