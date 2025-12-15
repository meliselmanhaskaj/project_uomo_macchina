using Progetto.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Progetto.Services.Shared;
using Progetto.Services.BuoniDigitali;

namespace Progetto.Services
{
    public class TemplateDbContext : DbContext
    {
        public TemplateDbContext()
        {
        }

        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
        {
            // I dati seed vengono caricati da Program.cs dopo la migrazione
        }

        public DbSet<User> Users { get; set; }
        
        // Entit� Buoni Digitali
        public DbSet<Azienda> Aziende { get; set; }
        public DbSet<Esercente> Esercenti { get; set; }
        public DbSet<Dipendente> Dipendenti { get; set; }
        public DbSet<Convenzione> Convenzioni { get; set; }
        public DbSet<UtilizzoConvenzione> UtilizziConvenzioni { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurazione indici per performance
            modelBuilder.Entity<Dipendente>()
                .HasIndex(d => d.UserId);

            modelBuilder.Entity<Dipendente>()
                .HasIndex(d => d.AziendaId);

            modelBuilder.Entity<Esercente>()
                .HasIndex(e => e.UserId);

            modelBuilder.Entity<Convenzione>()
                .HasIndex(c => new { c.AziendaId, c.EsercenteId });

            modelBuilder.Entity<UtilizzoConvenzione>()
                .HasIndex(u => u.CodiceUtilizzo)
                .IsUnique();

            modelBuilder.Entity<UtilizzoConvenzione>()
                .HasIndex(u => new { u.DipendenteId, u.DataRichiesta });

            modelBuilder.Entity<UtilizzoConvenzione>()
                .HasIndex(u => new { u.EsercenteId, u.DataRichiesta });

            // Configurazione relazioni con eliminazione a cascata disabilitata per evitare cicli
            modelBuilder.Entity<UtilizzoConvenzione>()
                .HasOne(u => u.Convenzione)
                .WithMany(c => c.Utilizzi)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UtilizzoConvenzione>()
                .HasOne(u => u.Dipendente)
                .WithMany(d => d.UtilizziConvenzioni)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UtilizzoConvenzione>()
                .HasOne(u => u.Esercente)
                .WithMany(e => e.UtilizziCertificati)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
