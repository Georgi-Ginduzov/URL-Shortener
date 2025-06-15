using Microsoft.AspNetCore.Mvc;
using URL_Shortener.Web.Services.Helpers;
using URL_Shortener.Web.Services.Interfaces;

namespace URL_Shortener.Web.Controllers
{
    public class RedirectController : Controller
    {
        private readonly IShorteningService _shortener;

        public RedirectController(IShorteningService shortener)
        {
            _shortener = shortener;
        }

        [HttpGet("/r/{shortCode}")]
        public async Task<IActionResult> To(string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode) || shortCode.Length > 11)
                return NotFound();

            var result = await _shortener.GetRedirectionUrl(shortCode, HttpContext);
            return result.Status switch
            {
                RedirectStatus.Found => Redirect(result.TargetUrl!),
                RedirectStatus.Expired => StatusCode(410),
                RedirectStatus.NotFound => NotFound(),
                _ => StatusCode(500)
            };
        }
    }
}
