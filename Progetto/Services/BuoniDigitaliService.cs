using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Progetto.Services.BuoniDigitali;

namespace Progetto.Services
{
    /// <summary>
    /// Servizio per la gestione delle convenzioni e certificazione degli utilizzi
    /// </summary>
    public class BuoniDigitaliService
    {
        private readonly TemplateDbContext _context;

        public BuoniDigitaliService(TemplateDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Genera un codice univoco per l'utilizzo della convenzione
        /// </summary>
        public string GeneraCodiceUtilizzo()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[6];
                rng.GetBytes(bytes);
                return $"BD-{BitConverter.ToString(bytes).Replace("-", "").ToUpper()}";
            }
        }

        /// <summary>
        /// Verifica se un dipendente può utilizzare una convenzione rispettando le regole
        /// </summary>
        public async Task<(bool Valida, string Errore)> VerificaValiditaConvenzione(
            Guid convenzioneId, 
            Guid dipendenteId, 
            decimal importoTotale)
        {
            var convenzione = await _context.Convenzioni
                .Include(c => c.Azienda)
                .Include(c => c.Esercente)
                .FirstOrDefaultAsync(c => c.Id == convenzioneId);

            if (convenzione == null)
                return (false, "Convenzione non trovata");

            if (!convenzione.Attiva)
                return (false, "Convenzione non attiva");

            if (convenzione.DataInizio > DateTime.UtcNow)
                return (false, $"Convenzione valida dal {convenzione.DataInizio:dd/MM/yyyy}");

            if (convenzione.DataFine.HasValue && convenzione.DataFine < DateTime.UtcNow)
                return (false, $"Convenzione scaduta il {convenzione.DataFine:dd/MM/yyyy}");

            var dipendente = await _context.Dipendenti
                .FirstOrDefaultAsync(d => d.Id == dipendenteId);

            if (dipendente == null)
                return (false, "Dipendente non trovato");

            if (!dipendente.Attivo)
                return (false, "Dipendente non attivo");

            if (dipendente.AziendaId != convenzione.AziendaId)
                return (false, "Convenzione non disponibile per la tua azienda");

            // Verifica importo minimo
            if (convenzione.ImportoMinimo.HasValue && importoTotale < convenzione.ImportoMinimo.Value)
                return (false, $"Importo minimo richiesto: €{convenzione.ImportoMinimo.Value:N2}");

            // Verifica numero massimo utilizzi per periodo
            if (convenzione.MaxUtilizziPerDipendente.HasValue && convenzione.PeriodoGiorni.HasValue)
            {
                var dataInizioperiodo = DateTime.UtcNow.AddDays(-convenzione.PeriodoGiorni.Value);
                var utilizziNelPeriodo = await _context.UtilizziConvenzioni
                    .CountAsync(u => 
                        u.ConvenzioneId == convenzioneId && 
                        u.DipendenteId == dipendenteId && 
                        u.Stato == StatoUtilizzo.Certificato &&
                        u.DataRichiesta >= dataInizioperiodo);

                if (utilizziNelPeriodo >= convenzione.MaxUtilizziPerDipendente.Value)
                    return (false, $"Limite di {convenzione.MaxUtilizziPerDipendente.Value} utilizzi ogni {convenzione.PeriodoGiorni.Value} giorni raggiunto");
            }

            return (true, null);
        }

        /// <summary>
        /// Calcola gli importi per un utilizzo di convenzione
        /// </summary>
        public (decimal ImportoSconto, decimal ImportoPagato, decimal ImportoRemunerazione) CalcolaImporti(
            Convenzione convenzione,
            decimal importoTotale)
        {
            // Calcola sconto al dipendente
            var importoSconto = importoTotale * (convenzione.PercentualeSconto / 100m);

            // Applica limite massimo sconto se presente
            if (convenzione.ImportoMassimoSconto.HasValue && importoSconto > convenzione.ImportoMassimoSconto.Value)
                importoSconto = convenzione.ImportoMassimoSconto.Value;

            var importoPagato = importoTotale - importoSconto;

            // Calcola remunerazione all'esercente (sulla base dell'importo totale)
            var importoRemunerazione = importoTotale * (convenzione.PercentualeRemunerazione / 100m);

            return (importoSconto, importoPagato, importoRemunerazione);
        }

        /// <summary>
        /// Crea una richiesta di utilizzo convenzione
        /// </summary>
        public async Task<UtilizzoConvenzione> CreaRichiestaUtilizzo(
            Guid convenzioneId,
            Guid dipendenteId,
            decimal importoTotale,
            string note = null)
        {
            // Verifica validità
            var (valida, errore) = await VerificaValiditaConvenzione(convenzioneId, dipendenteId, importoTotale);
            if (!valida)
                throw new InvalidOperationException(errore);

            var convenzione = await _context.Convenzioni
                .FirstOrDefaultAsync(c => c.Id == convenzioneId);

            // Calcola importi
            var (importoSconto, importoPagato, importoRemunerazione) = CalcolaImporti(convenzione, importoTotale);

            // Crea utilizzo
            var utilizzo = new UtilizzoConvenzione
            {
                ConvenzioneId = convenzioneId,
                DipendenteId = dipendenteId,
                EsercenteId = convenzione.EsercenteId,
                CodiceUtilizzo = GeneraCodiceUtilizzo(),
                ImportoTotale = importoTotale,
                ImportoSconto = importoSconto,
                ImportoPagato = importoPagato,
                ImportoRemunerazione = importoRemunerazione,
                Stato = StatoUtilizzo.Richiesto,
                DataRichiesta = DateTime.UtcNow,
                Note = note
            };

            _context.UtilizziConvenzioni.Add(utilizzo);
            await _context.SaveChangesAsync();

            return utilizzo;
        }

        /// <summary>
        /// Certifica un utilizzo di convenzione da parte dell'esercente
        /// </summary>
        public async Task<bool> CertificaUtilizzo(Guid utilizzoId, Guid esercenteId)
        {
            var utilizzo = await _context.UtilizziConvenzioni
                .Include(u => u.Convenzione)
                .FirstOrDefaultAsync(u => u.Id == utilizzoId);

            if (utilizzo == null)
                return false;

            if (utilizzo.EsercenteId != esercenteId)
                throw new UnauthorizedAccessException("Non sei autorizzato a certificare questo utilizzo");

            if (utilizzo.Stato != StatoUtilizzo.Richiesto)
                throw new InvalidOperationException($"Impossibile certificare un utilizzo nello stato {utilizzo.Stato}");

            utilizzo.Stato = StatoUtilizzo.Certificato;
            utilizzo.DataCertificazione = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Rifiuta un utilizzo di convenzione
        /// </summary>
        public async Task<bool> RifiutaUtilizzo(Guid utilizzoId, Guid esercenteId, string motivo)
        {
            var utilizzo = await _context.UtilizziConvenzioni
                .FirstOrDefaultAsync(u => u.Id == utilizzoId);

            if (utilizzo == null)
                return false;

            if (utilizzo.EsercenteId != esercenteId)
                throw new UnauthorizedAccessException("Non sei autorizzato a rifiutare questo utilizzo");

            if (utilizzo.Stato != StatoUtilizzo.Richiesto)
                throw new InvalidOperationException($"Impossibile rifiutare un utilizzo nello stato {utilizzo.Stato}");

            utilizzo.Stato = StatoUtilizzo.Rifiutato;
            utilizzo.MotivoRifiuto = motivo;
            utilizzo.DataCertificazione = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Recupera le convenzioni disponibili per un dipendente
        /// </summary>
        public IQueryable<Convenzione> GetConvenzioniDisponibili(Guid dipendenteId)
        {
            var dipendente = _context.Dipendenti.Find(dipendenteId);
            if (dipendente == null)
                return Enumerable.Empty<Convenzione>().AsQueryable();

            return _context.Convenzioni
                .Include(c => c.Esercente)
                .Include(c => c.Azienda)
                .Where(c => 
                    c.AziendaId == dipendente.AziendaId &&
                    c.Attiva &&
                    c.DataInizio <= DateTime.UtcNow &&
                    (!c.DataFine.HasValue || c.DataFine >= DateTime.UtcNow));
        }

        /// <summary>
        /// Recupera lo storico utilizzi di un dipendente
        /// </summary>
        public IQueryable<UtilizzoConvenzione> GetStoricoUtilizziDipendente(Guid dipendenteId)
        {
            return _context.UtilizziConvenzioni
                .Include(u => u.Convenzione)
                    .ThenInclude(c => c.Esercente)
                .Where(u => u.DipendenteId == dipendenteId)
                .OrderByDescending(u => u.DataRichiesta);
        }

        /// <summary>
        /// Recupera le richieste in attesa per un esercente
        /// </summary>
        public IQueryable<UtilizzoConvenzione> GetRichiesteInAttesa(Guid esercenteId)
        {
            return _context.UtilizziConvenzioni
                .Include(u => u.Convenzione)
                .Include(u => u.Dipendente)
                    .ThenInclude(d => d.User)
                .Where(u => u.EsercenteId == esercenteId && u.Stato == StatoUtilizzo.Richiesto)
                .OrderBy(u => u.DataRichiesta);
        }

        /// <summary>
        /// Recupera lo storico transazioni di un esercente
        /// </summary>
        public IQueryable<UtilizzoConvenzione> GetStoricoTransazioniEsercente(Guid esercenteId)
        {
            return _context.UtilizziConvenzioni
                .Include(u => u.Convenzione)
                .Include(u => u.Dipendente)
                    .ThenInclude(d => d.User)
                .Where(u => u.EsercenteId == esercenteId)
                .OrderByDescending(u => u.DataRichiesta);
        }

        /// <summary>
        /// Recupera tutte le aziende disponibili
        /// </summary>
        public System.Collections.Generic.List<Azienda> GetAziende()
        {
            return _context.Aziende.ToList();
        }
    }
}
