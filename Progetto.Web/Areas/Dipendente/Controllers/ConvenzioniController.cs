using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progetto.Services;
using Progetto.Web.Areas.Dipendente.ViewModels;
using Progetto.Web.Infrastructure;

namespace Progetto.Web.Areas.Dipendente.Controllers
{
    [Area("Dipendente")]
    [Authorize]
    public partial class ConvenzioniController : Controller
    {
        private readonly TemplateDbContext _context;
        private readonly BuoniDigitaliService _buoniService;

        public ConvenzioniController(TemplateDbContext context, BuoniDigitaliService buoniService)
        {
            _context = context;
            _buoniService = buoniService;
        }

        private async Task<Services.BuoniDigitali.Dipendente> GetCurrentDipendente()
        {
            var dipendenteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (dipendenteIdClaim == null || !Guid.TryParse(dipendenteIdClaim.Value, out Guid dipendenteId))
            {
                return null;
            }

            return await _context.Dipendenti
                .Include(d => d.User)
                .Include(d => d.Azienda)
                .FirstOrDefaultAsync(d => d.Id == dipendenteId);
        }

        // GET: Dipendente/Convenzioni
        public virtual async Task<IActionResult> Index()
        {
            var dipendente = await GetCurrentDipendente();
            if (dipendente == null)
            {
                return Content("Errore: impossibile trovare il profilo dipendente. Contatta l'amministratore.");
            }

            var convenzioni = await _buoniService.GetConvenzioniDisponibili(dipendente.Id)
                .Select(c => new ConvenzioneViewModel
                {
                    Id = c.Id,
                    Titolo = c.Titolo,
                    Descrizione = c.Descrizione,
                    NomeEsercente = c.Esercente.NomeAttivita,
                    CategoriaEsercente = c.Esercente.Categoria,
                    IndirizzoEsercente = c.Esercente.Indirizzo,
                    PercentualeSconto = c.PercentualeSconto,
                    ImportoMinimo = c.ImportoMinimo,
                    ImportoMassimoSconto = c.ImportoMassimoSconto,
                    MaxUtilizziPerDipendente = c.MaxUtilizziPerDipendente,
                    PeriodoGiorni = c.PeriodoGiorni,
                    DataInizio = c.DataInizio,
                    DataFine = c.DataFine
                })
                .ToListAsync();

            var viewModel = new ConvenzioniDisponibiliViewModel
            {
                Convenzioni = convenzioni,
                NomeDipendente = $"{dipendente.User.FirstName} {dipendente.User.LastName}",
                NomeAzienda = dipendente.Azienda.Nome
            };

            return View(viewModel);
        }

        // GET: Dipendente/Convenzioni/Richiedi/{id}
        public virtual async Task<IActionResult> Richiedi(Guid id)
        {
            var convenzione = await _context.Convenzioni
                .Include(c => c.Esercente)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (convenzione == null)
            {
                return NotFound();
            }

            var viewModel = new RichiestaUtilizzoViewModel
            {
                ConvenzioneId = convenzione.Id,
                TitoloConvenzione = convenzione.Titolo,
                NomeEsercente = convenzione.Esercente.NomeAttivita,
                PercentualeSconto = convenzione.PercentualeSconto,
                ImportoMinimo = convenzione.ImportoMinimo,
                ImportoMassimoSconto = convenzione.ImportoMassimoSconto
            };

            return View(viewModel);
        }

        // POST: Dipendente/Convenzioni/Richiedi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Richiedi(RichiestaUtilizzoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ricarica i dati della convenzione
                var conv = await _context.Convenzioni
                    .Include(c => c.Esercente)
                    .FirstOrDefaultAsync(c => c.Id == model.ConvenzioneId);
                
                model.TitoloConvenzione = conv.Titolo;
                model.NomeEsercente = conv.Esercente.NomeAttivita;
                model.PercentualeSconto = conv.PercentualeSconto;
                model.ImportoMinimo = conv.ImportoMinimo;
                model.ImportoMassimoSconto = conv.ImportoMassimoSconto;
                
                return View(model);
            }

            try
            {
                var dipendente = await GetCurrentDipendente();
                if (dipendente == null)
                {
                    ModelState.AddModelError("", "Dipendente non trovato");
                    return View(model);
                }

                var utilizzo = await _buoniService.CreaRichiestaUtilizzo(
                    model.ConvenzioneId,
                    dipendente.Id,
                    model.ImportoTotale,
                    model.Note
                );

                TempData.Success($"Richiesta creata con successo! Codice utilizzo: {utilizzo.CodiceUtilizzo}");
                return RedirectToAction(nameof(Storico));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                
                // Ricarica i dati della convenzione
                var conv = await _context.Convenzioni
                    .Include(c => c.Esercente)
                    .FirstOrDefaultAsync(c => c.Id == model.ConvenzioneId);
                
                model.TitoloConvenzione = conv.Titolo;
                model.NomeEsercente = conv.Esercente.NomeAttivita;
                model.PercentualeSconto = conv.PercentualeSconto;
                model.ImportoMinimo = conv.ImportoMinimo;
                model.ImportoMassimoSconto = conv.ImportoMassimoSconto;
                
                return View(model);
            }
        }

        // GET: Dipendente/Convenzioni/Storico
        public virtual async Task<IActionResult> Storico()
        {
            var dipendente = await GetCurrentDipendente();
            if (dipendente == null)
            {
                return Content("Errore: l'utente autenticato non è registrato come dipendente.");
            }

            var utilizzi = await _buoniService.GetStoricoUtilizziDipendente(dipendente.Id)
                .Select(u => new UtilizzoViewModel
                {
                    Id = u.Id,
                    CodiceUtilizzo = u.CodiceUtilizzo,
                    TitoloConvenzione = u.Convenzione.Titolo,
                    NomeEsercente = u.Convenzione.Esercente.NomeAttivita,
                    ImportoTotale = u.ImportoTotale,
                    ImportoSconto = u.ImportoSconto,
                    ImportoPagato = u.ImportoPagato,
                    Stato = u.Stato,
                    DataRichiesta = u.DataRichiesta,
                    DataCertificazione = u.DataCertificazione,
                    Note = u.Note,
                    MotivoRifiuto = u.MotivoRifiuto
                })
                .ToListAsync();

            var viewModel = new StoricoUtilizziViewModel
            {
                NomeDipendente = $"{dipendente.User.FirstName} {dipendente.User.LastName}",
                NomeAzienda = dipendente.Azienda.Nome,
                Utilizzi = utilizzi
            };

            return View(viewModel);
        }

        // API per calcolo importi in tempo reale
        [HttpPost]
        public virtual async Task<IActionResult> CalcolaImporti(Guid convenzioneId, decimal importoTotale)
        {
            var convenzione = await _context.Convenzioni.FindAsync(convenzioneId);
            
            if (convenzione == null)
            {
                return Json(new { success = false, error = "Convenzione non trovata" });
            }

            var (importoSconto, importoPagato, _) = _buoniService.CalcolaImporti(convenzione, importoTotale);

            return Json(new
            {
                success = true,
                importoSconto = importoSconto,
                importoPagato = importoPagato
            });
        }
    }
}
