namespace URL_Shortener.Web.Services.Dtos
{
    public class NewShortUrlDto
    {
        public string TargetUrl { get; set; }
        public Guid? SessionId { get; set; }
        public string? UserId { get; set; }
    }
}
