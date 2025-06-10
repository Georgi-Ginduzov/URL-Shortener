using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URL_Shortener.Web.Data.Entities
{
    public class ClickDetail
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [ForeignKey(nameof(Url))]
        public long UrlId { get; set; }
        public Url Url { get; set; }
        
        public DateTime Timestamp { get; set; }
        public string IPAdress { get; set; } = default!;
        public string? UserAgent { get; set; }
    }
}
