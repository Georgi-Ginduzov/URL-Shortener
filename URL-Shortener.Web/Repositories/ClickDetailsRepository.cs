using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using URL_Shortener.Web.Data;
using URL_Shortener.Web.Data.Entities;
using URL_Shortener.Web.Repositories.Interfaces;

namespace URL_Shortener.Web.Repositories
{
    public class ClickDetailsRepository : IClickDetailsRepository
    {
        private readonly ApplicationDbContext db;

        public ClickDetailsRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IList<ClickDetail>> GetByUrlId(long id) 
            => await db.ClickDetails
                .AsNoTracking()
                .Where(x => x.UrlId == id)
                .ToListAsync();

        public async Task<ClickDetail> AddAsync(long urlId)
        {
            var clickDetailObject = new ClickDetail()
            {
                Timestamp = DateTime.Now,
                UrlId = urlId,
            };

            var clickDetail = await db.ClickDetails.AddAsync(clickDetailObject);

            return clickDetail.Entity;
        }

        public async Task<ClickDetail> AddAsync(long urlId, string ipAddress)
        {
            var clickDetailObject = new ClickDetail()
            {
                Timestamp = DateTime.Now,
                UrlId = urlId,
                IPAdress = ipAddress,
            };

            var clickDetail = await db.ClickDetails.AddAsync(clickDetailObject);

            return clickDetail.Entity;
        }

        public async Task<ClickDetail> AddAsync(long urlId, string ipAddress, string userAgent)
        {
            var clickDetailObject = new ClickDetail()
            {
                Timestamp = DateTime.Now,
                UrlId = urlId,
                IPAdress = ipAddress,
                UserAgent = userAgent,
            };

            var clickDetail = await db.ClickDetails.AddAsync(clickDetailObject);

            return clickDetail.Entity;
        }

        public void Dispose()
        {
            db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
