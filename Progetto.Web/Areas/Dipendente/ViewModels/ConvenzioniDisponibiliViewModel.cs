using System;
using System.Collections.Generic;
using Progetto.Services.BuoniDigitali;

namespace Progetto.Web.Areas.Dipendente.ViewModels
{
    public class ConvenzioniDisponibiliViewModel
    {
        public List<ConvenzioneViewModel> Convenzioni { get; set; } = new List<ConvenzioneViewModel>();
        public string NomeDipendente { get; set; }
        public string NomeAzienda { get; set; }
    }

    public class ConvenzioneViewModel
    {
        public Guid Id { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public string NomeEsercente { get; set; }
        public string CategoriaEsercente { get; set; }
        public string IndirizzoEsercente { get; set; }
        public decimal PercentualeSconto { get; set; }
        public decimal? ImportoMinimo { get; set; }
        public decimal? ImportoMassimoSconto { get; set; }
        public int? MaxUtilizziPerDipendente { get; set; }
        public int? PeriodoGiorni { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
    }
}
