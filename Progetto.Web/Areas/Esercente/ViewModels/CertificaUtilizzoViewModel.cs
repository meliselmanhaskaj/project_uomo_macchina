using System;
using System.ComponentModel.DataAnnotations;

namespace Progetto.Web.Areas.Esercente.ViewModels
{
    public class CertificaUtilizzoViewModel
    {
        public Guid UtilizzoId { get; set; }
        public string CodiceUtilizzo { get; set; }
        public string TitoloConvenzione { get; set; }
        public string NomeDipendente { get; set; }
        public decimal ImportoTotale { get; set; }
        public decimal ImportoSconto { get; set; }
        public decimal ImportoPagato { get; set; }
        public decimal ImportoRemunerazione { get; set; }
        public string Note { get; set; }
    }

    public class RifiutaUtilizzoViewModel
    {
        public Guid UtilizzoId { get; set; }
        public string CodiceUtilizzo { get; set; }
        public string TitoloConvenzione { get; set; }
        public string NomeDipendente { get; set; }

        [Required(ErrorMessage = "Il motivo del rifiuto è obbligatorio")]
        [MaxLength(500, ErrorMessage = "Il motivo non può superare i 500 caratteri")]
        [Display(Name = "Motivo Rifiuto")]
        public string MotivoRifiuto { get; set; }
    }
}
