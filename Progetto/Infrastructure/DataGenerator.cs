using Progetto.Services.Shared;
using Progetto.Services.BuoniDigitali;
using System;
using System.Linq;
using Progetto.Services;

namespace Progetto.Infrastructure
{
    public class DataGenerator
    {
        public static void InitializeUsers(TemplateDbContext context)
        {
            if (context.Users.Any())
            {
                return;   // Data was already seeded
            }

            context.Users.AddRange(
                new User
                {
                    Id = Guid.Parse("3de6883f-9a0b-4667-aa53-0fbc52c4d300"), // Forced to specific Guid for tests
                    Email = "email1@test.it",
                    Password = "M0Cuk9OsrcS/rTLGf5SY6DUPqU2rGc1wwV2IL88GVGo=", // SHA-256 of text "Prova"
                    FirstName = "Nome1",
                    LastName = "Cognome1",
                    NickName = "Nickname1"
                },
                new User
                {
                    Id = Guid.Parse("a030ee81-31c7-47d0-9309-408cb5ac0ac7"), // Forced to specific Guid for tests
                    Email = "email2@test.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Nome2",
                    LastName = "Cognome2",
                    NickName = "Nickname2"
                },
                new User
                {
                    Id = Guid.Parse("bfdef48b-c7ea-4227-8333-c635af267354"), // Forced to specific Guid for tests
                    Email = "email3@test.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Nome3",
                    LastName = "Cognome3",
                    NickName = "Nickname3"
                },
                // Utenti per dipendenti
                new User
                {
                    Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                    Email = "mario.rossi@azienda1.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Mario",
                    LastName = "Rossi",
                    NickName = "MarioR"
                },
                new User
                {
                    Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                    Email = "lucia.verdi@azienda1.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Lucia",
                    LastName = "Verdi",
                    NickName = "LuciaV"
                },
                // Utenti per esercenti
                new User
                {
                    Id = Guid.Parse("e1111111-1111-1111-1111-111111111111"),
                    Email = "ristorante@italiano.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Giuseppe",
                    LastName = "Bianchi",
                    NickName = "RistoranteItaliano"
                },
                new User
                {
                    Id = Guid.Parse("e2222222-2222-2222-2222-222222222222"),
                    Email = "palestra@fitness.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Anna",
                    LastName = "Neri",
                    NickName = "PalestraFitness"
                });

            context.SaveChanges();
        }

        public static void InitializeBuoniDigitali(TemplateDbContext context)
        {
            if (context.Aziende.Any())
            {
                return;   // Data was already seeded
            }

            // Aziende
            var azienda1 = new Azienda
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                Nome = "Tech Solutions S.r.l.",
                PartitaIva = "12345678901",
                Indirizzo = "Via Roma 123, Milano",
                Telefono = "+39 02 12345678",
                Email = "info@techsolutions.it",
                Attiva = true,
                DataCreazione = DateTime.UtcNow
            };

            var azienda2 = new Azienda
            {
                Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                Nome = "Consulting Group S.p.A.",
                PartitaIva = "98765432109",
                Indirizzo = "Corso Vittorio Emanuele 45, Roma",
                Telefono = "+39 06 87654321",
                Email = "info@consultinggroup.it",
                Attiva = true,
                DataCreazione = DateTime.UtcNow
            };

            context.Aziende.AddRange(azienda1, azienda2);
            context.SaveChanges();

            // Esercenti
            var esercente1 = new Esercente
            {
                Id = Guid.Parse("e0111111-1111-1111-1111-111111111111"),
                UserId = Guid.Parse("e1111111-1111-1111-1111-111111111111"),
                NomeAttivita = "Ristorante Italiano",
                PartitaIva = "11223344556",
                Indirizzo = "Via Dante 10, Milano",
                Telefono = "+39 02 11223344",
                Email = "ristorante@italiano.it",
                Categoria = "Ristorazione",
                Attivo = true,
                DataRegistrazione = DateTime.UtcNow
            };

            var esercente2 = new Esercente
            {
                Id = Guid.Parse("e0222222-2222-2222-2222-222222222222"),
                UserId = Guid.Parse("e2222222-2222-2222-2222-222222222222"),
                NomeAttivita = "Palestra Fitness Plus",
                PartitaIva = "66778899001",
                Indirizzo = "Via dello Sport 25, Milano",
                Telefono = "+39 02 66778899",
                Email = "palestra@fitness.it",
                Categoria = "Sport e Benessere",
                Attivo = true,
                DataRegistrazione = DateTime.UtcNow
            };

            context.Esercenti.AddRange(esercente1, esercente2);
            context.SaveChanges();

            // Dipendenti
            var dipendente1 = new Dipendente
            {
                Id = Guid.Parse("d0111111-1111-1111-1111-111111111111"),
                UserId = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                AziendaId = azienda1.Id,
                Matricola = "TS001",
                Reparto = "Sviluppo Software",
                Attivo = true,
                DataAssunzione = DateTime.UtcNow.AddYears(-2),
                DataRegistrazione = DateTime.UtcNow
            };

            var dipendente2 = new Dipendente
            {
                Id = Guid.Parse("d0222222-2222-2222-2222-222222222222"),
                UserId = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                AziendaId = azienda1.Id,
                Matricola = "TS002",
                Reparto = "Marketing",
                Attivo = true,
                DataAssunzione = DateTime.UtcNow.AddYears(-1),
                DataRegistrazione = DateTime.UtcNow
            };

            context.Dipendenti.AddRange(dipendente1, dipendente2);
            context.SaveChanges();

            // Convenzioni
            var convenzione1 = new Convenzione
            {
                Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                AziendaId = azienda1.Id,
                EsercenteId = esercente1.Id,
                Titolo = "Sconto Pranzo Aziendale",
                Descrizione = "Sconto del 15% su tutti i pranzi dal luned� al venerd� per i dipendenti di Tech Solutions",
                PercentualeSconto = 15,
                PercentualeRemunerazione = 8,
                ImportoMinimo = 10.00m,
                ImportoMassimoSconto = 10.00m,
                MaxUtilizziPerDipendente = 20,
                PeriodoGiorni = 30,
                DataInizio = DateTime.UtcNow.AddMonths(-1),
                DataFine = DateTime.UtcNow.AddYears(1),
                Attiva = true,
                DataCreazione = DateTime.UtcNow
            };

            var convenzione2 = new Convenzione
            {
                Id = Guid.Parse("c2222222-2222-2222-2222-222222222222"),
                AziendaId = azienda1.Id,
                EsercenteId = esercente2.Id,
                Titolo = "Abbonamento Palestra Scontato",
                Descrizione = "Sconto del 20% su abbonamenti mensili e annuali per i dipendenti di Tech Solutions",
                PercentualeSconto = 20,
                PercentualeRemunerazione = 10,
                ImportoMinimo = 30.00m,
                MaxUtilizziPerDipendente = 1,
                PeriodoGiorni = 30,
                DataInizio = DateTime.UtcNow.AddMonths(-1),
                DataFine = DateTime.UtcNow.AddYears(1),
                Attiva = true,
                DataCreazione = DateTime.UtcNow
            };

            context.Convenzioni.AddRange(convenzione1, convenzione2);
            context.SaveChanges();

            // Utilizzi convenzioni (storico transazioni)
            var utilizzo1 = new UtilizzoConvenzione
            {
                Id = Guid.Parse("01111111-1111-1111-1111-111111111111"),
                ConvenzioneId = convenzione1.Id,
                DipendenteId = Guid.Parse("d0111111-1111-1111-1111-111111111111"),
                EsercenteId = esercente1.Id,
                CodiceUtilizzo = "LUNCH001",
                ImportoTotale = 25.00m,
                ImportoSconto = 3.75m,
                ImportoPagato = 21.25m,
                ImportoRemunerazione = 2.00m,
                DataRichiesta = DateTime.UtcNow.AddDays(-5),
                DataCertificazione = DateTime.UtcNow.AddDays(-5),
                Stato = StatoUtilizzo.Certificato,
                Note = "Pranzo di lavoro - Menu del giorno"
            };

            var utilizzo2 = new UtilizzoConvenzione
            {
                Id = Guid.Parse("02222222-2222-2222-2222-222222222222"),
                ConvenzioneId = convenzione1.Id,
                DipendenteId = Guid.Parse("d0111111-1111-1111-1111-111111111111"),
                EsercenteId = esercente1.Id,
                CodiceUtilizzo = "LUNCH002",
                ImportoTotale = 30.00m,
                ImportoSconto = 4.50m,
                ImportoPagato = 25.50m,
                ImportoRemunerazione = 2.40m,
                DataRichiesta = DateTime.UtcNow.AddDays(-3),
                DataCertificazione = DateTime.UtcNow.AddDays(-3),
                Stato = StatoUtilizzo.Certificato,
                Note = "Pranzo business con cliente"
            };

            var utilizzo3 = new UtilizzoConvenzione
            {
                Id = Guid.Parse("03333333-3333-3333-3333-333333333333"),
                ConvenzioneId = convenzione1.Id,
                DipendenteId = Guid.Parse("d0222222-2222-2222-2222-222222222222"),
                EsercenteId = esercente1.Id,
                CodiceUtilizzo = "LUNCH003",
                ImportoTotale = 18.00m,
                ImportoSconto = 2.70m,
                ImportoPagato = 15.30m,
                ImportoRemunerazione = 1.44m,
                DataRichiesta = DateTime.UtcNow.AddDays(-2),
                DataCertificazione = DateTime.UtcNow.AddDays(-2),
                Stato = StatoUtilizzo.Certificato,
                Note = "Pranzo veloce"
            };

            var utilizzo4 = new UtilizzoConvenzione
            {
                Id = Guid.Parse("04444444-4444-4444-4444-444444444444"),
                ConvenzioneId = convenzione2.Id,
                DipendenteId = Guid.Parse("d0111111-1111-1111-1111-111111111111"),
                EsercenteId = esercente2.Id,
                CodiceUtilizzo = "GYM001",
                ImportoTotale = 50.00m,
                ImportoSconto = 10.00m,
                ImportoPagato = 40.00m,
                ImportoRemunerazione = 5.00m,
                DataRichiesta = DateTime.UtcNow.AddDays(-10),
                DataCertificazione = DateTime.UtcNow.AddDays(-10),
                Stato = StatoUtilizzo.Certificato,
                Note = "Abbonamento mensale palestra"
            };

            var utilizzo5 = new UtilizzoConvenzione
            {
                Id = Guid.Parse("05555555-5555-5555-5555-555555555555"),
                ConvenzioneId = convenzione1.Id,
                DipendenteId = Guid.Parse("d0111111-1111-1111-1111-111111111111"),
                EsercenteId = esercente1.Id,
                CodiceUtilizzo = "LUNCH004",
                ImportoTotale = 22.00m,
                ImportoSconto = 3.30m,
                ImportoPagato = 18.70m,
                ImportoRemunerazione = 1.76m,
                DataRichiesta = DateTime.UtcNow.AddHours(-2),
                Stato = StatoUtilizzo.Richiesto,
                Note = "In attesa di validazione"
            };

            context.UtilizziConvenzioni.AddRange(utilizzo1, utilizzo2, utilizzo3, utilizzo4, utilizzo5);
            context.SaveChanges();
        }
    }
}
