using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using URL_Shortener.Web.Data;
using URL_Shortener.Web.Data.Entities;
using URL_Shortener.Web.Data.Entities.Enums;
using URL_Shortener.Web.Repositories.Interfaces;
using URL_Shortener.Web.Utilities;

namespace URL_Shortener.Web.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private const string urlCreationSql = """
            DECLARE
                @url     nvarchar(2048)      = {0},
                @hash    nvarchar(64)        = {1},
                @session uniqueidentifier    = {2},
                @userId  nvarchar(450)       = {3},
                @mode    int                 = {4};

            MERGE dbo.URLs WITH (HOLDLOCK) AS tgt
            USING (SELECT @hash AS HashedTargetURL) AS src
            ON tgt.HashedTargetURL = src.HashedTargetURL
            WHEN NOT MATCHED THEN
              INSERT (SessionId, UserId, TargetURL, HashedTargetURL,
                      CreatedAt,  ExpiresAt,    AnalitycsMode)
              VALUES (@session, @userId, @url, @hash,
                      SYSUTCDATETIME(),
                      DATEADD(month, 1, SYSUTCDATETIME()),
                      @mode)
            --  ShortenedURL is **not** in the column list: SQL Server computes it
            OUTPUT inserted.*;      -- gives us Id, ShortenedURL, everything
            """;
        private readonly ApplicationDbContext db;

        public UrlRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async ValueTask<Url> AddNewUrlAsync(string targetUrl, Guid sessionId) =>
            await AddNewUrlCoreAsync(targetUrl, sessionId: sessionId, userId: null);

        public async ValueTask<Url> AddNewUrlAsync(string targetUrl, string userId) =>
            await AddNewUrlCoreAsync(targetUrl, sessionId: null, userId: userId);

        public async ValueTask<Url> AddNewUrlAsync(string targetUrl, string userId, Guid sessionId) =>
           await AddNewUrlCoreAsync(targetUrl, sessionId, userId);

        public async Task<Url?> GetByIdAsync(long id)
            => await db.URLs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Url?> GetByIdAndUserIdAsync(long id, string userId)
            => await db.URLs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        private async ValueTask<Url> AddNewUrlCoreAsync(string rawUrl, Guid? sessionId, string? userId, AnalitycsType analitycsType = AnalitycsType.None)
        {
            var url = rawUrl.Trim();
            
            var local = db.URLs.Local // cache check instead!!
                        .FirstOrDefault(u => u.TargetURL == url
                                          && u.SessionId == sessionId
                                          && u.UserId == userId);
            if (local != null) 
                return local;

            var entity = new Url
            {
                SessionId = sessionId,
                UserId = userId,
                TargetURL = url,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMonths(1),
                AnalitycsMode = analitycsType
            };
            db.URLs.Add(entity);

            try
            {
                await db.SaveChangesAsync();
                return entity;                  
            }
            catch (DbUpdateException ex) when (IsDuplicate(ex))
            {
                db.Entry(entity).State = EntityState.Detached;

                var hash = Cryptography.HashSha256(url);

                var existingUrl = await db.URLs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.HashedTargetURL == hash
                                                && u.SessionId == sessionId && u.UserId == userId);

                return existingUrl!;
            }
        }

        private static bool IsDuplicate(DbUpdateException ex)
            => ex.InnerException is SqlException sql 
                    && (sql.Number is 2601 or 2627);
    }
}
