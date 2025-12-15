using System;
using System.Collections.Generic;

namespace Progetto.Web.Areas.Esercente.ViewModels
{
    public class StoricoTransazioniViewModel
    {
        public List<TransazioneViewModel> Transazioni { get; set; } = new List<TransazioneViewModel>();
        public int TotaleTransazioni { get; set; }
        public int TotaleCertificate { get; set; }
        public int TotaleRifiutate { get; set; }
        public decimal TotaleIncassato { get; set; }
        public decimal TotaleRemunerazione { get; set; }
        public decimal TotaleRicavi { get; set; }
    }

    public class TransazioneViewModel
    {
        public Guid Id { get; set; }
        public string CodiceUtilizzo { get; set; }
        public string TitoloConvenzione { get; set; }
        public string NomeDipendente { get; set; }
        public string Azienda { get; set; }
        public decimal ImportoTotale { get; set; }
        public decimal ImportoSconto { get; set; }
        public decimal ImportoPagato { get; set; }
        public decimal ImportoRemunerazione { get; set; }
        public string Stato { get; set; }
        public DateTime DataRichiesta { get; set; }
        public DateTime? DataCertificazione { get; set; }
        public string Note { get; set; }
        public string MotivoRifiuto { get; set; }
    }
}
