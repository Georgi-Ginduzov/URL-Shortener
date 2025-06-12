namespace URL_Shortener.Web.ExternalSystems.SafeBrowsing.Models
{
    public class ThreatInfo
    {
        public IList<string> ThreatTypes { get; set; } = new[]
          { "MALWARE", "SOCIAL_ENGINEERING", "UNWANTED_SOFTWARE", "POTENTIALLY_HARMFUL_APPLICATION" };
        public IList<string> PlatformTypes { get; set; } = new[] { "ANY_PLATFORM" };
        public IList<string> ThreatEntryTypes { get; set; } = new[] { "URL" };
        public IList<ThreatEntry> ThreatEntries { get; set; } = Array.Empty<ThreatEntry>();
    }

}
