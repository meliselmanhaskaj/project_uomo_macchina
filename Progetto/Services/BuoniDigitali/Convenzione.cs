using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progetto.Services.BuoniDigitali
{
    /// <summary>
    /// Rappresenta una convenzione tra un'azienda e un esercente
    /// </summary>
    public class Convenzione
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid AziendaId { get; set; }

        [Required]
        public Guid EsercenteId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titolo { get; set; }

        [MaxLength(1000)]
        public string Descrizione { get; set; }

        /// <summary>
        /// Percentuale di sconto offerta al dipendente (es: 10, 15, 20)
        /// </summary>
        [Required]
        [Range(0, 100)]
        public decimal PercentualeSconto { get; set; }

        /// <summary>
        /// Percentuale di remunerazione riconosciuta all'esercente dall'azienda (es: 5, 8, 10)
        /// </summary>
        [Required]
        [Range(0, 100)]
        public decimal PercentualeRemunerazione { get; set; }

        /// <summary>
        /// Importo minimo per poter utilizzare la convenzione
        /// </summary>
        public decimal? ImportoMinimo { get; set; }

        /// <summary>
        /// Importo massimo dello sconto applicabile
        /// </summary>
        public decimal? ImportoMassimoSconto { get; set; }

        /// <summary>
        /// Numero massimo di utilizzi per dipendente per periodo
        /// </summary>
        public int? MaxUtilizziPerDipendente { get; set; }

        /// <summary>
        /// Periodo in giorni per il calcolo del massimo utilizzi (es: 30 per mensile)
        /// </summary>
        public int? PeriodoGiorni { get; set; }

        public DateTime DataInizio { get; set; }

        public DateTime? DataFine { get; set; }

        public bool Attiva { get; set; } = true;

        public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("AziendaId")]
        public virtual Azienda Azienda { get; set; }

        [ForeignKey("EsercenteId")]
        public virtual Esercente Esercente { get; set; }

        public virtual ICollection<UtilizzoConvenzione> Utilizzi { get; set; } = new List<UtilizzoConvenzione>();
    }
}
