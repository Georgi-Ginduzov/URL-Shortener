using System.Runtime.CompilerServices;
using URL_Shortener.Web.Data;
using URL_Shortener.Web.Repositories.Interfaces;

namespace URL_Shortener.Web.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;

        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;

            ClickDetailsRepository = new ClickDetailsRepository(db);
            UrlRepository = new UrlRepository(db);
            SessionRepository = new SessionRepository(db);
        }
        
        public IUrlRepository UrlRepository { get; }
        public IClickDetailsRepository ClickDetailsRepository { get; }
        public ISessionRepository SessionRepository { get; }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public void Dispose()
        {
            db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
