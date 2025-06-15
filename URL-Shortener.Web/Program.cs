using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Web.Data;
using URL_Shortener.Web.ExternalSystems.SafeBrowsing.Models;
using URL_Shortener.Web.Repositories;
using URL_Shortener.Web.Repositories.Interfaces;
using URL_Shortener.Web.Services;
using URL_Shortener.Web.Services.Interfaces;

namespace URL_Shortener.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.Configure<SafeBrowsingOptions>(builder.Configuration.GetSection("SafeBrowsing"));

            builder.Services.AddSession(opts =>
            {
                opts.Cookie.Name = ".UrlShort.Session";
                opts.IdleTimeout = TimeSpan.FromMinutes(30);
                opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                opts.Cookie.HttpOnly = true;
                opts.Cookie.SameSite = SameSiteMode.Lax;
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUrlRepository, UrlRepository>();
            builder.Services.AddScoped<IClickDetailsRepository, ClickDetailsRepository>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHttpClient<IUrlSecurityService, UrlSecurityService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(10));
            builder.Services.AddScoped<IShorteningService, ShorteningService>();
            builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // app.MapControllerRoute(
            //     name: "default",
            //     pattern: "{controller=Home}/{action=Index}/{id?}");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "redirect",
                    pattern: "r/{shortCode}",
                    defaults: new { controller = "Redirect", action = "To" });
            });
            app.MapRazorPages();

            app.Run();
        }
    }
}
