using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Progetto.Web.Controllers
{
    public partial class WelcomeController : Controller
    {
        public virtual async Task<IActionResult> Index()
        {
            // Previeni il caching della pagina
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Se l'utente è autenticato, fai logout automaticamente
            if (HttpContext.User != null && HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        // Pagina 404 personalizzata
        [Route("NotFound")]
        public virtual IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("~/Features/Home/NotFound.cshtml");
        }
    }
}
