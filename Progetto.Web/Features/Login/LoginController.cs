using Progetto.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Progetto.Services.Shared;
using System.Threading.Tasks;
using Progetto.Infrastructure;
using Progetto.Services;
using System.Linq;

namespace Progetto.Web.Features.Login
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Alerts]
    [ModelStateToTempData]
    public partial class LoginController : Controller
    {
        public static string LoginErrorModelStateKey = "LoginError";
        private readonly SharedService _sharedService;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly TemplateDbContext _context;

        public LoginController(SharedService sharedService, IStringLocalizer<SharedResource> sharedLocalizer, TemplateDbContext context)
        {
            _sharedService = sharedService;
            _sharedLocalizer = sharedLocalizer;
            _context = context;
        }

        private async Task<(bool Success, string ErrorMessage, ActionResult Result)> LoginAndRedirect(UserDetailDTO utente)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, utente.Email)
            };

            string redirectUrl = null;

            // Controlla se l'utente è un dipendente
            var dipendente = _context.Dipendenti.FirstOrDefault(d => d.UserId == utente.Id);
            if (dipendente != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, dipendente.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, string.Format("{0} {1}", utente.FirstName, utente.LastName)));
                claims.Add(new Claim(ClaimTypes.Role, "Dipendente"));
                redirectUrl = "/Dipendente/Convenzioni";
            }
            else
            {
                // Controlla se l'utente è un esercente
                var esercente = _context.Esercenti.FirstOrDefault(e => e.UserId == utente.Id);
                if (esercente != null)
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, esercente.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, esercente.NomeAttivita));
                    claims.Add(new Claim(ClaimTypes.Role, "Esercente"));
                    redirectUrl = "/Esercente/Transazioni";
                }
                else
                {
                    // L'utente non è né dipendente né esercente
                    return (false, "Non sei registrato come dipendente o esercente. Registrati prima di accedere.", null);
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                IsPersistent = false,
            });

            return (true, null, Redirect(redirectUrl));
        }

        [HttpGet]
        public virtual async Task<IActionResult> Login(string tipo)
        {
            // Se l'utente è già autenticato, reindirizzalo alla sua dashboard
            if (HttpContext.User != null && HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                var roleClaim = HttpContext.User.FindFirst(ClaimTypes.Role);
                
                if (idClaim != null && Guid.TryParse(idClaim.Value, out Guid id) && roleClaim != null)
                {
                    // Controlla se è un dipendente valido
                    if (roleClaim.Value == "Dipendente")
                    {
                        var dipendente = _context.Dipendenti.FirstOrDefault(d => d.Id == id);
                        if (dipendente != null)
                        {
                            return RedirectToAction(MVC.Dipendente.Convenzioni.Index());
                        }
                    }

                    // Controlla se è un esercente valido
                    if (roleClaim.Value == "Esercente")
                    {
                        var esercente = _context.Esercenti.FirstOrDefault(e => e.Id == id);
                        if (esercente != null)
                        {
                            return RedirectToAction(MVC.Esercente.Transazioni.Index());
                        }
                    }
                }

                // Se arriviamo qui, l'utente è autenticato ma non ha un profilo valido
                // Facciamo logout automatico
                await HttpContext.SignOutAsync();
            }

            var model = new LoginViewModel
            {
                TipoAccesso = tipo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async virtual Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Trova l'utente per email
                    var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                    
                    // Verifica password con hash
                    if (user == null || !PasswordHasher.VerifyPassword(model.Password, user.Password))
                    {
                        ModelState.AddModelError(LoginErrorModelStateKey, "Email o password errate");
                        return View(model);
                    }

                    // Verifica che il tipo di utente corrisponda al tipo di accesso richiesto
                    if (!string.IsNullOrEmpty(model.TipoAccesso))
                    {
                        if (model.TipoAccesso.ToLower() == "dipendente")
                        {
                            var dipendente = _context.Dipendenti.FirstOrDefault(d => d.UserId == user.Id);
                            if (dipendente == null)
                            {
                                ModelState.AddModelError(LoginErrorModelStateKey, "Email o password errate");
                                return View(model);
                            }
                        }
                        else if (model.TipoAccesso.ToLower() == "esercente")
                        {
                            var esercente = _context.Esercenti.FirstOrDefault(e => e.UserId == user.Id);
                            if (esercente == null)
                            {
                                ModelState.AddModelError(LoginErrorModelStateKey, "Email o password errate");
                                return View(model);
                            }
                        }
                    }
                    
                    var utente = new UserDetailDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };

                    var loginResult = await LoginAndRedirect(utente);
                    
                    if (!loginResult.Success)
                    {
                        // L'utente non è registrato come dipendente/esercente
                        ModelState.AddModelError(LoginErrorModelStateKey, loginResult.ErrorMessage);
                        return View(model);
                    }

                    return loginResult.Result;
                }
                catch (LoginException e)
                {
                    ModelState.AddModelError(LoginErrorModelStateKey, e.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Redirect("/");
        }
    }
}
