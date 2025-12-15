using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using Progetto.Web.Infrastructure;

namespace Progetto.Web.Areas
{
    [Authorize]
    [Alerts]
    [ModelStateToTempData]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AuthenticatedBaseController : Controller
    {
        public AuthenticatedBaseController() { }

        protected IdentitaViewModel Identita
        {
            get
            {
                return (IdentitaViewModel)ViewData[IdentitaViewModel.VIEWDATA_IDENTITACORRENTE_KEY];
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                if (context.HttpContext != null && context.HttpContext.User != null && context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var email = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault()?.Value;
                    var nome = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
                    var ruolo = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault()?.Value;
                    
                    ViewData[IdentitaViewModel.VIEWDATA_IDENTITACORRENTE_KEY] = new IdentitaViewModel
                    {
                        EmailUtenteCorrente = email,
                        NomeOrganizzazione = nome,
                        Ruolo = ruolo
                    };
                }
                else
                {
                    HttpContext.SignOutAsync();
                    this.SignOut();

                    context.Result = new RedirectResult(context.HttpContext.Request.GetEncodedUrl());
                    Alerts.AddError(this, "L'utente non possiede i diritti per visualizzare la risorsa richiesta");
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
