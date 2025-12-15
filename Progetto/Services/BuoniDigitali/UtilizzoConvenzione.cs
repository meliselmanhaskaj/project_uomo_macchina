using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progetto.Services.BuoniDigitali
{
    /// <summary>
    /// Rappresenta l'utilizzo certificato di una convenzione da parte di un dipendente presso un esercente
    /// </summary>
    public class UtilizzoConvenzione
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ConvenzioneId { get; set; }

        [Required]
        public Guid DipendenteId { get; set; }

        [Required]
        public Guid EsercenteId { get; set; }

        /// <summary>
        /// Codice univoco generato per questo utilizzo (per la validazione)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CodiceUtilizzo { get; set; }

        /// <summary>
        /// Importo totale della transazione (prima dello sconto)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportoTotale { get; set; }

        /// <summary>
        /// Importo dello sconto applicato al dipendente
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportoSconto { get; set; }

        /// <summary>
        /// Importo pagato dal dipendente
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportoPagato { get; set; }

        /// <summary>
        /// Importo della remunerazione dovuta all'esercente dall'azienda
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ImportoRemunerazione { get; set; }

        /// <summary>
        /// Stato: Richiesto, Certificato, Rifiutato, Annullato
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Stato { get; set; } = StatoUtilizzo.Richiesto;

        public DateTime DataRichiesta { get; set; } = DateTime.UtcNow;

        public DateTime? DataCertificazione { get; set; }

        [MaxLength(500)]
        public string Note { get; set; }

        [MaxLength(500)]
        public string MotivoRifiuto { get; set; }

        // Navigation properties
        [ForeignKey("ConvenzioneId")]
        public virtual Convenzione Convenzione { get; set; }

        [ForeignKey("DipendenteId")]
        public virtual Dipendente Dipendente { get; set; }

        [ForeignKey("EsercenteId")]
        public virtual Esercente Esercente { get; set; }
    }

    /// <summary>
    /// Stati possibili per un utilizzo di convenzione
    /// </summary>
    public static class StatoUtilizzo
    {
        public const string Richiesto = "Richiesto";
        public const string Certificato = "Certificato";
        public const string Rifiutato = "Rifiutato";
        public const string Annullato = "Annullato";
    }
}
