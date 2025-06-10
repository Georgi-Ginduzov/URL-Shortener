using System.ComponentModel.DataAnnotations;

namespace URL_Shortener.Web.Data.Entities
{
    public class Session
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeenAt { get; set; }

        public ICollection<Url> Urls { get; set; }
    }
}
