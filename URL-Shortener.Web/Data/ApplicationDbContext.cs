using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Reflection.Emit;
using URL_Shortener.Web.Data.Entities;

namespace URL_Shortener.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Url> URLs { get; set; }
        public DbSet<ClickDetail> ClickDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Url>(e =>
            {
                // 1️⃣  SHA-256 hash stored column (keeps index small)
                e.Property(u => u.HashedTargetURL)
                 .HasComputedColumnSql("CONVERT(varchar(64), HASHBYTES('SHA2_256',[TargetURL]), 2)", stored: true);

                // 2️⃣  Base-62 slug from identity – SQL Server does it
                e.Property(u => u.ShortenedURL)
                 .HasComputedColumnSql("dbo.Base62Encode([Id])", stored: true)
                 .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                // 3️⃣  Alternate-key constraints for dedupe
                e.HasIndex(u => new { u.SessionId, u.HashedTargetURL })
                 .IsUnique()
                 .HasFilter("[UserId] IS NULL");

                e.HasIndex(u => new { u.UserId, u.HashedTargetURL })
                 .IsUnique()
                 .HasFilter("[SessionId] IS NULL");
            });

            builder.Entity<ClickDetail>()
                .HasOne(c => c.Url)
                .WithMany(u => u.ClickDetails)
                .HasForeignKey(c => c.UrlId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
