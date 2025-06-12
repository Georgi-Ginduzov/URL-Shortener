namespace URL_Shortener.Web.ExternalSystems.SafeBrowsing.Models
{
    public class SafeBrowsingRequest
    {
        public ClientInfo Client { get; set; } = new ClientInfo();
        public ThreatInfo ThreatInfo { get; set; } = new ThreatInfo();
    }

}
