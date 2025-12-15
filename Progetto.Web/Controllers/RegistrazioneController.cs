using Microsoft.AspNetCore.Mvc;
using Progetto.Services;
using Progetto.Services.BuoniDigitali;
using Progetto.Services.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Progetto.Infrastructure;

namespace Progetto.Web.Controllers
{
    public partial class RegistrazioneController : Controller
    {
        private readonly BuoniDigitaliService _service;
        private readonly TemplateDbContext _context;

        public RegistrazioneController(BuoniDigitaliService service, TemplateDbContext context)
        {
            _service = service;
            _context = context;
        }

        public virtual IActionResult Dipendente()
        {
            var aziende = _service.GetAziende();
            return View(new RegistrazioneDipendenteViewModel
            {
                Aziende = aziende
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Dipendente(RegistrazioneDipendenteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Aziende = _service.GetAziende();
                return View(model);
            }

            if (model.Password != model.ConfermaPassword)
            {
                ModelState.AddModelError("ConfermaPassword", "Le password non coincidono");
                model.Aziende = _service.GetAziende();
                return View(model);
            }

            // Verifica se l'email esiste già
            var emailEsistente = _context.Users.Any(u => u.Email == model.Email);
            if (emailEsistente)
            {
                ModelState.AddModelError("Email", "Questa email è già registrata");
                model.Aziende = _service.GetAziende();
                return View(model);
            }

            // Crea l'utente
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = model.Nome,
                LastName = model.Cognome,
                Email = model.Email,
                Password = PasswordHasher.HashPassword(model.Password)
            };
            _context.Users.Add(user);

            // Crea il dipendente
            var dipendente = new Dipendente
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                AziendaId = model.AziendaId,
                Matricola = string.Format("MAT{0:D6}", new Random().Next(1, 999999)),
                DataAssunzione = DateTime.UtcNow,
                Attivo = true
            };
            _context.Dipendenti.Add(dipendente);
            _context.SaveChanges();

            // Login automatico dopo registrazione
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, dipendente.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, string.Format("{0} {1}", user.FirstName, user.LastName)),
                new Claim(ClaimTypes.Role, "Dipendente")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Convenzioni", new { area = "Dipendente" });
        }

        public virtual IActionResult Esercente()
        {
            return View(new RegistrazioneEsercenteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Esercente(RegistrazioneEsercenteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfermaPassword)
            {
                ModelState.AddModelError("ConfermaPassword", "Le password non coincidono");
                return View(model);
            }

            // Verifica se l'email esiste già
            var emailEsistente = _context.Users.Any(u => u.Email == model.Email);
            if (emailEsistente)
            {
                ModelState.AddModelError("Email", "Questa email è già registrata");
                return View(model);
            }

            // Crea l'utente
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = model.NomeEsercizio,
                LastName = "",
                Email = model.Email,
                Password = PasswordHasher.HashPassword(model.Password)
            };
            _context.Users.Add(user);

            // Crea l'esercente
            var esercente = new Esercente
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                NomeAttivita = model.NomeEsercizio,
                Indirizzo = model.Indirizzo,
                Categoria = model.Categoria,
                Email = model.Email,
                Telefono = model.Telefono,
                PartitaIva = model.PartitaIva,
                Attivo = true
            };
            _context.Esercenti.Add(esercente);
            _context.SaveChanges();

            // Crea una convenzione di default per il nuovo esercente con tutte le aziende
            var aziende = _context.Aziende.ToList();
            foreach (var azienda in aziende)
            {
                var convenzione = new Convenzione
                {
                    Id = Guid.NewGuid(),
                    AziendaId = azienda.Id,
                    EsercenteId = esercente.Id,
                    Titolo = $"Convenzione {model.NomeEsercizio}",
                    Descrizione = $"Convenzione con {model.NomeEsercizio} - {model.Categoria}. Indirizzo: {model.Indirizzo}",
                    PercentualeSconto = model.PercentualeSconto,
                    PercentualeRemunerazione = model.PercentualeRemunerazione,
                    ImportoMinimo = model.ImportoMinimo,
                    ImportoMassimoSconto = model.ImportoMassimoSconto,
                    MaxUtilizziPerDipendente = model.MaxUtilizziPerDipendente,
                    PeriodoGiorni = model.PeriodoGiorni,
                    DataInizio = DateTime.UtcNow,
                    DataFine = DateTime.UtcNow.AddMonths(model.DurataMesi),
                    Attiva = true,
                    DataCreazione = DateTime.UtcNow
                };
                _context.Convenzioni.Add(convenzione);
            }
            _context.SaveChanges();

            // Login automatico dopo registrazione
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, esercente.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, esercente.NomeAttivita),
                new Claim(ClaimTypes.Role, "Esercente")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Transazioni", new { area = "Esercente" });
        }
    }

    public class RegistrazioneDipendenteViewModel
    {
        [Required(ErrorMessage = "Il campo Nome è obbligatorio")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "Il campo Cognome è obbligatorio")]
        public string Cognome { get; set; }
        
        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [EmailAddress(ErrorMessage = "Inserire un indirizzo email valido")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [MinLength(4, ErrorMessage = "La password deve essere di almeno 4 caratteri")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Il campo Conferma Password è obbligatorio")]
        [Compare("Password", ErrorMessage = "Le password non corrispondono")]
        public string ConfermaPassword { get; set; }
        
        [Required(ErrorMessage = "Selezionare un'azienda")]
        public Guid AziendaId { get; set; }
        
        public System.Collections.Generic.List<Azienda> Aziende { get; set; }
    }

    public class RegistrazioneEsercenteViewModel
    {
        [Required(ErrorMessage = "Il campo Nome Esercizio è obbligatorio")]
        public string NomeEsercizio { get; set; }
        
        [Required(ErrorMessage = "Il campo Indirizzo è obbligatorio")]
        public string Indirizzo { get; set; }
        
        [Required(ErrorMessage = "Il campo Categoria è obbligatorio")]
        public string Categoria { get; set; }
        
        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [EmailAddress(ErrorMessage = "Inserire un indirizzo email valido")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [MinLength(4, ErrorMessage = "La password deve essere di almeno 4 caratteri")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Il campo Conferma Password è obbligatorio")]
        [Compare("Password", ErrorMessage = "Le password non corrispondono")]
        public string ConfermaPassword { get; set; }
        
        [Required(ErrorMessage = "Il campo Telefono è obbligatorio")]
        public string Telefono { get; set; }
        
        [Required(ErrorMessage = "Il campo Partita IVA è obbligatorio")]
        public string PartitaIva { get; set; }

        // Parametri convenzione
        [Required(ErrorMessage = "La percentuale di sconto è obbligatoria")]
        [Range(0, 100, ErrorMessage = "Lo sconto deve essere tra 0 e 100")]
        public decimal PercentualeSconto { get; set; } = 10;

        [Required(ErrorMessage = "La percentuale di remunerazione è obbligatoria")]
        [Range(0, 100, ErrorMessage = "La remunerazione deve essere tra 0 e 100")]
        public decimal PercentualeRemunerazione { get; set; } = 5;

        [Required(ErrorMessage = "L'importo minimo è obbligatorio")]
        [Range(0, 10000, ErrorMessage = "L'importo minimo deve essere positivo")]
        public decimal ImportoMinimo { get; set; } = 10;

        [Required(ErrorMessage = "Lo sconto massimo è obbligatorio")]
        [Range(0, 10000, ErrorMessage = "Lo sconto massimo deve essere positivo")]
        public decimal ImportoMassimoSconto { get; set; } = 50;

        [Required(ErrorMessage = "Il limite di utilizzi è obbligatorio")]
        [Range(1, 100, ErrorMessage = "Il limite deve essere almeno 1")]
        public int MaxUtilizziPerDipendente { get; set; } = 3;

        [Required(ErrorMessage = "Il periodo in giorni è obbligatorio")]
        [Range(1, 365, ErrorMessage = "Il periodo deve essere tra 1 e 365 giorni")]
        public int PeriodoGiorni { get; set; } = 30;

        [Required(ErrorMessage = "La durata in mesi è obbligatoria")]
        [Range(1, 60, ErrorMessage = "La durata deve essere tra 1 e 60 mesi")]
        public int DurataMesi { get; set; } = 12;
    }
}
