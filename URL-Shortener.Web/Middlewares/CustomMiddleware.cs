using Azure.Core;
using Azure;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Web.Data.Entities;

namespace URL_Shortener.Web.Middlewares
{
    public class CustomMiddleware
    {
        public void AssignSession()
        {
            // In your middleware or base controller:
            /*var sessionCookie = Request.Cookies["SessionId"];
            if (string.IsNullOrEmpty(sessionCookie) || !Guid.TryParse(sessionCookie, out var sessionGuid))
            {
                sessionGuid = Guid.NewGuid();
                Response.Cookies.Append("SessionId", sessionGuid.ToString(),
                    new CookieOptions { HttpOnly = true, Expires = DateTimeOffset.UtcNow.AddDays(30) });

                // Insert into Session table:
                dbContext.Sessions.Add(new Session
                {
                    SessionId = sessionGuid,
                    CreatedAt = DateTime.UtcNow,
                    LastSeenAt = DateTime.UtcNow
                });
                await dbContext.SaveChangesAsync();
            }
            else
            {
                // update LastSeenAt
                var sessionRow = await dbContext.Sessions.FindAsync(sessionGuid);
                if (sessionRow != null)
                {
                    sessionRow.LastSeenAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }
            }
*/
        }
    }
}
