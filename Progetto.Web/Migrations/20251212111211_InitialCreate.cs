using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Progetto.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aziende",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PartitaIva = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Indirizzo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Attiva = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aziende", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    NickName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dipendenti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AziendaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Matricola = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Reparto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataAssunzione = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataRegistrazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dipendenti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dipendenti_Aziende_AziendaId",
                        column: x => x.AziendaId,
                        principalTable: "Aziende",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dipendenti_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Esercenti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeAttivita = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PartitaIva = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Indirizzo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataRegistrazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Esercenti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Esercenti_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Convenzioni",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AziendaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EsercenteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titolo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PercentualeSconto = table.Column<decimal>(type: "TEXT", nullable: false),
                    PercentualeRemunerazione = table.Column<decimal>(type: "TEXT", nullable: false),
                    ImportoMinimo = table.Column<decimal>(type: "TEXT", nullable: true),
                    ImportoMassimoSconto = table.Column<decimal>(type: "TEXT", nullable: true),
                    MaxUtilizziPerDipendente = table.Column<int>(type: "INTEGER", nullable: true),
                    PeriodoGiorni = table.Column<int>(type: "INTEGER", nullable: true),
                    DataInizio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFine = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Attiva = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convenzioni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Convenzioni_Aziende_AziendaId",
                        column: x => x.AziendaId,
                        principalTable: "Aziende",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Convenzioni_Esercenti_EsercenteId",
                        column: x => x.EsercenteId,
                        principalTable: "Esercenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilizziConvenzioni",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConvenzioneId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DipendenteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EsercenteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CodiceUtilizzo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ImportoTotale = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportoSconto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportoPagato = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImportoRemunerazione = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stato = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataRichiesta = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataCertificazione = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MotivoRifiuto = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizziConvenzioni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilizziConvenzioni_Convenzioni_ConvenzioneId",
                        column: x => x.ConvenzioneId,
                        principalTable: "Convenzioni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UtilizziConvenzioni_Dipendenti_DipendenteId",
                        column: x => x.DipendenteId,
                        principalTable: "Dipendenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UtilizziConvenzioni_Esercenti_EsercenteId",
                        column: x => x.EsercenteId,
                        principalTable: "Esercenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Convenzioni_AziendaId_EsercenteId",
                table: "Convenzioni",
                columns: new[] { "AziendaId", "EsercenteId" });

            migrationBuilder.CreateIndex(
                name: "IX_Convenzioni_EsercenteId",
                table: "Convenzioni",
                column: "EsercenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Dipendenti_AziendaId",
                table: "Dipendenti",
                column: "AziendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dipendenti_UserId",
                table: "Dipendenti",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Esercenti_UserId",
                table: "Esercenti",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizziConvenzioni_CodiceUtilizzo",
                table: "UtilizziConvenzioni",
                column: "CodiceUtilizzo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtilizziConvenzioni_ConvenzioneId",
                table: "UtilizziConvenzioni",
                column: "ConvenzioneId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizziConvenzioni_DipendenteId_DataRichiesta",
                table: "UtilizziConvenzioni",
                columns: new[] { "DipendenteId", "DataRichiesta" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilizziConvenzioni_EsercenteId_DataRichiesta",
                table: "UtilizziConvenzioni",
                columns: new[] { "EsercenteId", "DataRichiesta" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UtilizziConvenzioni");

            migrationBuilder.DropTable(
                name: "Convenzioni");

            migrationBuilder.DropTable(
                name: "Dipendenti");

            migrationBuilder.DropTable(
                name: "Esercenti");

            migrationBuilder.DropTable(
                name: "Aziende");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
