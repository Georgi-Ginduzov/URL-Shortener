using URL_Shortener.Web.Data.Entities;

namespace URL_Shortener.Web.Repositories.Interfaces
{
    public interface IUrlRepository
    {
        ValueTask<Url> AddNewUrlAsync(string targetUrl, Guid sessionId);
        ValueTask<Url> AddNewUrlAsync(string targetUrl, string userId);
        ValueTask<Url> AddNewUrlAsync(string targetUrl, string userId, Guid sessionId);
        Task<Url?> GetByIdAndUserIdAsync(long id, string userId);
        Task<Url?> GetByIdAsync(long id);
    }
}