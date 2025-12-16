using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progetto.Services;
using Progetto.Services.BuoniDigitali;
using Progetto.Web.Areas.Esercente.ViewModels;
using Progetto.Web.Infrastructure;

namespace Progetto.Web.Areas.Esercente.Controllers
{
    [Area("Esercente")]
    [Authorize]
    [Alerts]
    public partial class TransazioniController : Controller
    {
        private readonly TemplateDbContext _context;
        private readonly BuoniDigitaliService _buoniService;

        public TransazioniController(TemplateDbContext context, BuoniDigitaliService buoniService)
        {
            _context = context;
            _buoniService = buoniService;
        }

        private async Task<Services.BuoniDigitali.Esercente> GetCurrentEsercente()
        {
            var esercenteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (esercenteIdClaim == null || !Guid.TryParse(esercenteIdClaim.Value, out Guid esercenteId))
            {
                return null;
            }

            return await _context.Esercenti
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == esercenteId);
        }

        // GET: Esercente/Transazioni
        public virtual async Task<IActionResult> Index()
        {
            var esercente = await GetCurrentEsercente();
            if (esercente == null)
            {
                return Content("Errore: impossibile trovare il profilo esercente. Contatta l'amministratore.");
            }

            var richieste = await _buoniService.GetRichiesteInAttesa(esercente.Id)
                .Select(u => new RichiestaViewModel
                {
                    Id = u.Id,
                    CodiceUtilizzo = u.CodiceUtilizzo,
                    TitoloConvenzione = u.Convenzione.Titolo,
                    NomeDipendente = u.Dipendente.User.FirstName + " " + u.Dipendente.User.LastName,
                    EmailDipendente = u.Dipendente.User.Email,
                    Azienda = u.Dipendente.Azienda.Nome,
                    ImportoTotale = u.ImportoTotale,
                    ImportoSconto = u.ImportoSconto,
                    ImportoPagato = u.ImportoPagato,
                    ImportoRemunerazione = u.ImportoRemunerazione,
                    DataRichiesta = u.DataRichiesta,
                    Note = u.Note
                })
                .ToListAsync();

            var viewModel = new RichiesteInAttesaViewModel
            {
                Richieste = richieste,
                NomeEsercente = esercente.NomeAttivita
            };

            return View(viewModel);
        }

        // GET: Esercente/Transazioni/Certifica/{id}
        public virtual async Task<IActionResult> Certifica(Guid id)
        {
            var utilizzo = await _context.UtilizziConvenzioni
                .Include(u => u.Convenzione)
                .Include(u => u.Dipendente)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (utilizzo == null)
            {
                return NotFound();
            }

            var viewModel = new CertificaUtilizzoViewModel
            {
                UtilizzoId = utilizzo.Id,
                CodiceUtilizzo = utilizzo.CodiceUtilizzo,
                TitoloConvenzione = utilizzo.Convenzione.Titolo,
                NomeDipendente = $"{utilizzo.Dipendente.User.FirstName} {utilizzo.Dipendente.User.LastName}",
                ImportoTotale = utilizzo.ImportoTotale,
                ImportoSconto = utilizzo.ImportoSconto,
                ImportoPagato = utilizzo.ImportoPagato,
                ImportoRemunerazione = utilizzo.ImportoRemunerazione,
                Note = utilizzo.Note
            };

            return View(viewModel);
        }

        // POST: Esercente/Transazioni/Certifica
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Certifica")]
        public virtual async Task<IActionResult> ConfermaUtilizzo(Guid utilizzoId)
        {
            try
            {
                var esercente = await GetCurrentEsercente();
                if (esercente == null)
                {
                    Alerts.AddError(this, "Esercente non trovato", 5000);
                    return RedirectToAction(nameof(Index));
                }

                // Recupera informazioni per il messaggio
                var utilizzo = await _context.UtilizziConvenzioni
                    .Include(u => u.Convenzione)
                    .FirstOrDefaultAsync(u => u.Id == utilizzoId);
                
                var success = await _buoniService.CertificaUtilizzo(utilizzoId, esercente.Id);

                if (success)
                {
                    var nomeConvenzione = utilizzo?.Convenzione?.Titolo ?? "convenzione";
                    Alerts.AddSuccess(this, $"Hai confermato la certificazione della convenzione '{nomeConvenzione}'", 8000);
                }
                else
                {
                    Alerts.AddError(this, "Utilizzo non trovato", 5000);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Alerts.AddError(this, ex.Message, 6000);
            }
            catch (InvalidOperationException ex)
            {
                Alerts.AddError(this, ex.Message, 6000);
            }
            catch (Exception ex)
            {
                Alerts.AddError(this, $"Errore durante la certificazione: {ex.Message}", 6000);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Esercente/Transazioni/Rifiuta/{id}
        public virtual async Task<IActionResult> Rifiuta(Guid id)
        {
            var utilizzo = await _context.UtilizziConvenzioni
                .Include(u => u.Convenzione)
                .Include(u => u.Dipendente)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (utilizzo == null)
            {
                return NotFound();
            }

            var viewModel = new RifiutaUtilizzoViewModel
            {
                UtilizzoId = utilizzo.Id,
                CodiceUtilizzo = utilizzo.CodiceUtilizzo,
                TitoloConvenzione = utilizzo.Convenzione.Titolo,
                NomeDipendente = $"{utilizzo.Dipendente.User.FirstName} {utilizzo.Dipendente.User.LastName}"
            };

            return View(viewModel);
        }

        // POST: Esercente/Transazioni/Rifiuta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Rifiuta(RifiutaUtilizzoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var esercente = await GetCurrentEsercente();
                if (esercente == null)
                {
                    Alerts.AddError(this, "Esercente non trovato", 5000);
                    return RedirectToAction(nameof(Index));
                }

                // Recupera informazioni per il messaggio
                var utilizzo = await _context.UtilizziConvenzioni
                    .Include(u => u.Convenzione)
                    .FirstOrDefaultAsync(u => u.Id == model.UtilizzoId);
                
                var success = await _buoniService.RifiutaUtilizzo(model.UtilizzoId, esercente.Id, model.MotivoRifiuto);

                if (success)
                {
                    var nomeConvenzione = utilizzo?.Convenzione?.Titolo ?? "convenzione";
                    Alerts.AddError(this, $"Hai rifiutato l'utilizzo della convenzione '{nomeConvenzione}'", 8000);
                }
                else
                {
                    Alerts.AddError(this, "Utilizzo non trovato", 5000);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Alerts.AddError(this, ex.Message, 6000);
            }
            catch (InvalidOperationException ex)
            {
                Alerts.AddError(this, ex.Message, 6000);
            }
            catch (Exception ex)
            {
                Alerts.AddError(this, $"Errore durante il rifiuto: {ex.Message}", 6000);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Esercente/Transazioni/Storico
        public virtual async Task<IActionResult> Storico(int count = 10)
        {
            var esercente = await GetCurrentEsercente();
            if (esercente == null)
            {
                return Content("Errore: l'utente autenticato non è registrato come esercente.");
            }

            var tutteTransazioni = await _buoniService.GetStoricoTransazioniEsercente(esercente.Id)
                .Select(u => new TransazioneViewModel
                {
                    Id = u.Id,
                    CodiceUtilizzo = u.CodiceUtilizzo,
                    TitoloConvenzione = u.Convenzione.Titolo,
                    NomeDipendente = u.Dipendente.User.FirstName + " " + u.Dipendente.User.LastName,
                    Azienda = u.Dipendente.Azienda.Nome,
                    ImportoTotale = u.ImportoTotale,
                    ImportoSconto = u.ImportoSconto,
                    ImportoPagato = u.ImportoPagato,
                    ImportoRemunerazione = u.ImportoRemunerazione,
                    Stato = u.Stato,
                    DataRichiesta = u.DataRichiesta,
                    DataCertificazione = u.DataCertificazione,
                    Note = u.Note,
                    MotivoRifiuto = u.MotivoRifiuto
                })
                .ToListAsync();

            var transazioni = tutteTransazioni.Take(count).ToList();

            var viewModel = new StoricoTransazioniViewModel
            {
                Transazioni = transazioni,
                TotaleTransazioni = tutteTransazioni.Count,
                TransazioniVisualizzate = transazioni.Count,
                TotaleCertificate = tutteTransazioni.Count(t => t.Stato == StatoUtilizzo.Certificato),
                TotaleRifiutate = tutteTransazioni.Count(t => t.Stato == StatoUtilizzo.Rifiutato),
                TotaleIncassato = tutteTransazioni
                    .Where(t => t.Stato == StatoUtilizzo.Certificato)
                    .Sum(t => t.ImportoPagato),
                TotaleRemunerazione = tutteTransazioni
                    .Where(t => t.Stato == StatoUtilizzo.Certificato)
                    .Sum(t => t.ImportoRemunerazione),
                TotaleRicavi = tutteTransazioni
                    .Where(t => t.Stato == StatoUtilizzo.Certificato)
                    .Sum(t => t.ImportoPagato + t.ImportoRemunerazione)
            };

            return View(viewModel);
        }
    }
}
