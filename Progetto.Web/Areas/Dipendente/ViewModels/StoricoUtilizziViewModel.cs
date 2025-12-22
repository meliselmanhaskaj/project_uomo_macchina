using System;
using System.Collections.Generic;

namespace Progetto.Web.Areas.Dipendente.ViewModels
{
    public class StoricoUtilizziViewModel
    {
        public string NomeDipendente { get; set; }
        public string NomeAzienda { get; set; }
        public List<UtilizzoViewModel> Utilizzi { get; set; } = new List<UtilizzoViewModel>();
        public int TotaleUtilizzi { get; set; }
        public int UtilizziVisualizzati { get; set; }
        public int TotaleCertificati { get; set; }
        public int TotaleRifiutati { get; set; }
        public decimal TotaleRisparmiato { get; set; }
    }

    public class UtilizzoViewModel
    {
        public Guid Id { get; set; }
        public string CodiceUtilizzo { get; set; }
        public string TitoloConvenzione { get; set; }
        public string NomeEsercente { get; set; }
        public decimal ImportoTotale { get; set; }
        public decimal ImportoSconto { get; set; }
        public decimal ImportoPagato { get; set; }
        public string Stato { get; set; }
        public DateTime DataRichiesta { get; set; }
        public DateTime? DataCertificazione { get; set; }
        public string Note { get; set; }
        public string MotivoRifiuto { get; set; }
    }
}
