using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Web.Data;
using URL_Shortener.Web.Data.Entities;
using URL_Shortener.Web.Data.Entities.Enums;
using URL_Shortener.Web.Repositories.Interfaces;
using URL_Shortener.Web.Utilities;

namespace URL_Shortener.Web.Repositories
{
    public class UrlRepository : IUrlRepository
    {
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

        public async Task<IList<Url>> GetAllByUserIdAsync(string userId)
            => await db.URLs
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

        public async Task<Url> Update(Url url)
        {
            var dbUrl = await this.GetByIdAsync(url.Id);

            dbUrl!.TargetURL = url.TargetURL;
            dbUrl.AnalitycsMode = url.AnalitycsMode;
            dbUrl.ExpiresAt = url.ExpiresAt;

            return dbUrl;
        }

        public async Task<Url> Update(long id, DateTime expirationDate)
        {
            var url = await this.GetByIdAsync(id);

            url!.ExpiresAt = expirationDate;
            
            return url;
        }

        public async Task<Url> Update(long id, AnalitycsType analitycs)
        {
            var url = await this.GetByIdAsync(id);
            url!.AnalitycsMode = analitycs;

            return url;
        }
        
        public async Task<Url> Update(long id, string targetUrl)
        {
            var url = await this.GetByIdAsync(id);
            url!.TargetURL = targetUrl;
            url!.HashedTargetURL = Cryptography.HashSha256(targetUrl);

            return url;
        }

        

        private async ValueTask<Url> AddNewUrlCoreAsync(string rawUrl, Guid? sessionId, string? userId, AnalitycsType analitycsType = AnalitycsType.None)
        {
            var url = rawUrl.Trim();
            
            var local = db.URLs.Local // cache check instead!!
                        .FirstOrDefault(u => u.TargetURL == url
                                          && u.SessionId == sessionId
                                          && u.UserId == userId);
            if (local is not null) 
                return local;

            var hash = Cryptography.HashSha256(url);
            var existingUrl = await db.URLs
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.HashedTargetURL == hash
                                            && (u.SessionId == sessionId || u.UserId == userId));
            if (existingUrl is not null)
                return existingUrl!;

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

            return entity;                  
        }

        public async Task RemoveAsync(long id)
        {
            var entity = await db.URLs.FindAsync(id);
            if (entity is null) 
                return;

            db.URLs.Remove(entity);
        }

        public void Remove(Url url)
        {
            db.URLs.Remove(url);
        }

        public void Dispose()
        {
            db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
