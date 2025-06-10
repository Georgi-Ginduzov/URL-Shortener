using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using URL_Shortener.Web.Models;
using URL_Shortener.Web.Models.UrlVMs;
using URL_Shortener.Web.Services.Interfaces;

namespace URL_Shortener.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor http;
        private readonly IShorteningService shortener;
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger,
            IShorteningService shortener, IHttpContextAccessor http)
        {
            this.logger = logger;
            this.shortener = shortener;
            this.http = http;
            http.HttpContext.Session.SetString("Init", new Guid().ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Index() => View(new ShortenUrlVm());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ShortenUrlVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var urlEntity = await shortener.ShortenUrl(vm.TargetUrl, http.HttpContext!);
                
                var uri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                
                vm.ShortUrl = uri.ToString() + "r/" + urlEntity.ShortUrl;
                vm.TargetUrl = urlEntity.TargetUrl;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Shorten failed");
                vm.Error = "Sorry, the link could not be shortened right now.";
            }

            return View(vm);
        }
    }
}
