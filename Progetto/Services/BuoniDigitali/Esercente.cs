using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Progetto.Services.Shared;

namespace Progetto.Services.BuoniDigitali
{
    /// <summary>
    /// Rappresenta un esercente convenzionato che accetta i buoni digitali
    /// </summary>
    public class Esercente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string NomeAttivita { get; set; }

        [Required]
        [MaxLength(16)]
        public string PartitaIva { get; set; }

        [MaxLength(500)]
        public string Indirizzo { get; set; }

        [MaxLength(100)]
        public string Telefono { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Categoria { get; set; } // Es: Ristorante, Palestra, Cinema, ecc.

        public bool Attivo { get; set; } = true;

        public DateTime DataRegistrazione { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<Convenzione> Convenzioni { get; set; } = new List<Convenzione>();
        public virtual ICollection<UtilizzoConvenzione> UtilizziCertificati { get; set; } = new List<UtilizzoConvenzione>();
    }
}
