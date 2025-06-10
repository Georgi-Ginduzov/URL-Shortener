using URL_Shortener.Web.Data.Entities;

namespace URL_Shortener.Web.Repositories.Interfaces
{
    public interface IClickDetailsRepository
    {
        Task<ClickDetail> AddAsync(long urlId);
        Task<ClickDetail> AddAsync(long urlId, string ipAddress);
        Task<ClickDetail> AddAsync(long urlId, string ipAddress, string userAgent);
    }
}