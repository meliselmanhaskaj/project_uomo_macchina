using System;
using System.Collections.Generic;

namespace Progetto.Web.Areas.Esercente.ViewModels
{
    public class RichiesteInAttesaViewModel
    {
        public List<RichiestaViewModel> Richieste { get; set; } = new List<RichiestaViewModel>();
        public string NomeEsercente { get; set; }
    }

    public class RichiestaViewModel
    {
        public Guid Id { get; set; }
        public string CodiceUtilizzo { get; set; }
        public string TitoloConvenzione { get; set; }
        public string NomeDipendente { get; set; }
        public string EmailDipendente { get; set; }
        public string Azienda { get; set; }
        public decimal ImportoTotale { get; set; }
        public decimal ImportoSconto { get; set; }
        public decimal ImportoPagato { get; set; }
        public decimal ImportoRemunerazione { get; set; }
        public DateTime DataRichiesta { get; set; }
        public string Note { get; set; }
    }
}
