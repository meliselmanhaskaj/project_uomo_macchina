using System;
using System.ComponentModel.DataAnnotations;

namespace Progetto.Web.Areas.Dipendente.ViewModels
{
    public class RichiestaUtilizzoViewModel
    {
        public Guid ConvenzioneId { get; set; }
        
        public string TitoloConvenzione { get; set; }
        public string NomeEsercente { get; set; }
        public decimal PercentualeSconto { get; set; }
        public decimal? ImportoMinimo { get; set; }
        public decimal? ImportoMassimoSconto { get; set; }

        [Required(ErrorMessage = "L'importo totale è obbligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "L'importo deve essere maggiore di zero")]
        [Display(Name = "Importo Totale (€)")]
        public decimal ImportoTotale { get; set; }

        [Display(Name = "Note")]
        [MaxLength(500, ErrorMessage = "Le note non possono superare i 500 caratteri")]
        public string Note { get; set; }

        // Campi calcolati (read-only)
        public decimal? ImportoScontoCalcolato { get; set; }
        public decimal? ImportoPagatoCalcolato { get; set; }
    }
}
