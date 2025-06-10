namespace URL_Shortener.Web.Services.Helpers
{
    public static class SessionExtensions
    {
        private const string CacheKey = "__SessionGuid";

        public static Guid GetOrCreateGuid(this ISession session)
        {
            if (session.Id != null)
                return Guid.Parse(session.Id);

            Guid g = Guid.NewGuid();
            session.Set(CacheKey, g.ToByteArray());
            return g;
        }
    }
}
