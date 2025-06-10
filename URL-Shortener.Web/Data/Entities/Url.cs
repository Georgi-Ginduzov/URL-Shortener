using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using URL_Shortener.Web.Data.Entities.Enums;

namespace URL_Shortener.Web.Data.Entities
{
    public class Url
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(Session))]
        public Guid? SessionId { get; set; }
        public Session? Session { get; set; }

        [ForeignKey(nameof(IdentityUser))]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        [MaxLength(11)]
        public string ShortenedURL { get; set; } = default!;
        
        [MaxLength(64)]
        public string HashedTargetURL { get; set; } = default!;
        public string TargetURL { get; set; } = default!;
        public AnalitycsType AnalitycsMode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        [NotMapped]
        public ICollection<ClickDetail> ClickDetails { get; set; }
    }
}
