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
            var session = db.Sessions.Local.FirstOrDefault(s => s.Id == sessionId);
            if (session is not null)
            {
                Touch(session);
                return session;
            }

            session = await db.Sessions.FindAsync(sessionId);
            if (session is not null)
            {
                Touch(session);
                return session;
            }

            var timestamp = DateTime.Now;

            session = new Session
            {
                Id = sessionId,
                CreatedAt = timestamp,
                LastSeenAt = timestamp,
            };
            var createdSession = db.Sessions.Add(session);

            return createdSession.Entity;
        }

        /// <summary>
        /// Updates LastSeenAt property to the current time directly in the database
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task TouchAsync(Guid sessionId)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($"""
            UPDATE Sessions
            SET LastSeenAt = {DateTime.Now}
            WHERE Id = {sessionId}
            """);
        }

        /// <summary>
        /// Changes the property of last seen at to the current time
        /// </summary>
        /// <param name="session"></param>
        public void Touch(Session session) => session.LastSeenAt = DateTime.Now;

        public void Dispose()
        {
            db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
