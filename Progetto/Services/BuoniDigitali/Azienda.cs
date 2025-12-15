using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progetto.Services.BuoniDigitali
{
    /// <summary>
    /// Rappresenta un'azienda che offre convenzioni ai propri dipendenti
    /// </summary>
    public class Azienda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(16)]
        public string PartitaIva { get; set; }

        [MaxLength(500)]
        public string Indirizzo { get; set; }

        [MaxLength(100)]
        public string Telefono { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        public bool Attiva { get; set; } = true;

        public DateTime DataCreazione { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Dipendente> Dipendenti { get; set; } = new List<Dipendente>();
        public virtual ICollection<Convenzione> Convenzioni { get; set; } = new List<Convenzione>();
    }
}
