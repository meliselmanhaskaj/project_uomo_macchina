using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Progetto.Services.Shared;

namespace Progetto.Services.BuoniDigitali
{
    /// <summary>
    /// Rappresenta un dipendente che può utilizzare le convenzioni della propria azienda
    /// </summary>
    public class Dipendente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid AziendaId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Matricola { get; set; }

        [MaxLength(100)]
        public string Reparto { get; set; }

        public bool Attivo { get; set; } = true;

        public DateTime DataAssunzione { get; set; }

        public DateTime DataRegistrazione { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("AziendaId")]
        public virtual Azienda Azienda { get; set; }

        public virtual ICollection<UtilizzoConvenzione> UtilizziConvenzioni { get; set; } = new List<UtilizzoConvenzione>();
    }
}
