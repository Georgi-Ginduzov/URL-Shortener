
namespace URL_Shortener.Web.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUrlRepository UrlRepository { get; }
        IClickDetailsRepository ClickDetailsRepository { get; }
        ISessionRepository SessionRepository { get; }

        Task SaveAsync();
    }
}
