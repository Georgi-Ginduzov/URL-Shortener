using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using URL_Shortener.Web.Data.Entities;
using URL_Shortener.Web.Data.Entities.Enums;
using URL_Shortener.Web.Models.UrlVMs;
using URL_Shortener.Web.Repositories.Interfaces;
using URL_Shortener.Web.Services.Interfaces;

namespace URL_Shortener.Web.Controllers
{
    [Authorize]
    public class UrlsController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IShorteningService shorteningService;

        public UrlsController(IUnitOfWork unitOfWork, IShorteningService shorteningService)
        {
            this.unitOfWork = unitOfWork;
            this.shorteningService = shorteningService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            var urls = await unitOfWork.UrlRepository.GetAllByUserIdAsync(userId);
            var urlVMs = new List<UrlReadOnlyVM>();
            var uri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            foreach (var url in urls)
                urlVMs.Add(new UrlReadOnlyVM(url, uri));

            return View(urlVMs);
        }

        public async Task<IActionResult> Details(long id)
        {
            var userId = User.Identity.GetUserId();
            var url = await unitOfWork.UrlRepository.GetByIdAndUserIdAsync(id, userId);
            var uri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

            if (url is null)
                return NotFound();

            var urlVM = new UrlReadOnlyVM(url, uri);

            return View(urlVM);
        }
        
        public ActionResult Create() => RedirectToAction("Index", "Home");

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id)
        {
            var userId = User.Identity.GetUserId();
            var url = await unitOfWork.UrlRepository.GetByIdAndUserIdAsync(id, userId);
            if (url == null)
                return NotFound();

            var uri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

            var urlVM = new UrlReadOnlyVM(url, uri);
            return View(urlVM);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, UrlReadOnlyVM readOnlyVM)
        {
            try
            {
                var url = new Url()
                {
                    Id = id,
                    TargetURL = readOnlyVM.TargetUrl,
                    AnalitycsMode = readOnlyVM.Analitycs,
                    ExpiresAt = readOnlyVM.ExpiresAt,
                };

                await unitOfWork.UrlRepository.Update(url);
                await unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(); // Probably add error
            }
        }
        
        [HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, DateTime expirationDate)
        {
            try
            {
                await unitOfWork.UrlRepository.Update(id, expirationDate);
                await unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(); // Probably add error
            }
        }
        
        [HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AnalitycsType analytics)
        {
            try
            {
                await unitOfWork.UrlRepository.Update(id, analytics);
                await unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(); // Probably add error
            }
        }
        
        [HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, string targetUrl)
        {
            try
            {
                await unitOfWork.UrlRepository.Update(id, targetUrl);
                await unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(); // Probably add error
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                await unitOfWork.UrlRepository.RemoveAsync(id);
                await unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
