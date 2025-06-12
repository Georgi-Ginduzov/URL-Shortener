using URL_Shortener.Web.Data.Entities;
using URL_Shortener.Web.Data;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Web.Repositories.Interfaces;

namespace URL_Shortener.Web.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext db;

        public SessionRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<Session> GetOrCreateAsync(Guid sessionId)
        {
            var session = await db.Sessions.FindAsync(sessionId);
            if (session != null)
                return session;

            var timestamp = DateTime.Now;

            session = new Session
            {
                Id = sessionId,
                CreatedAt = timestamp,
                LastSeenAt = timestamp,
            };
            db.Sessions.Add(session);
            await db.SaveChangesAsync();
            return session;
        }

        public async Task TouchAsync(Guid sessionId)
        {
            // A plain UPDATE avoids loading the entity
            await db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE Sessions
            SET LastSeenAt = {DateTime.Now}
            WHERE Id = {sessionId}
            """);
        }
    }
}
