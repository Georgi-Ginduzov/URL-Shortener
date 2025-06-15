using URL_Shortener.Web.Data.Entities;
using URL_Shortener.Web.Data.Entities.Enums;

namespace URL_Shortener.Web.Repositories.Interfaces
{
    public interface IUrlRepository : IRepository
    {
        ValueTask<Url> AddNewUrlAsync(string targetUrl, Guid sessionId);
        ValueTask<Url> AddNewUrlAsync(string targetUrl, string userId);
        ValueTask<Url> AddNewUrlAsync(string targetUrl, string userId, Guid sessionId);
        Task<IList<Url>> GetAllByUserIdAsync(string userId);
        Task<Url?> GetByIdAndUserIdAsync(long id, string userId);
        Task<Url?> GetByIdAsync(long id);
        void Remove(Url url);
        Task RemoveAsync(long id);
        Task<Url> Update(long id, DateTime expirationDate);
        Task<Url> Update(long id, AnalitycsType analitycs);
        Task<Url> Update(long id, string targetUrl);
        Task<Url> Update(Url url);
    }
}