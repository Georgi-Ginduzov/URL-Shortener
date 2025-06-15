using URL_Shortener.Web.Data.Entities;

namespace URL_Shortener.Web.Repositories.Interfaces
{
    public interface ISessionRepository : IRepository
    {
        Task<Session> GetOrCreateAsync(Guid sessionId);
        Task TouchAsync(Guid sessionId);
    }
}