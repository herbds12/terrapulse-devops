using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TerraPulse.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AreasMonitoradas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Regiao = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Cultura = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TipoArea = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TamanhoHectares = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaSincronizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreasMonitoradas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Perfil = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertasAmbientais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaMonitoradaId = table.Column<int>(type: "int", nullable: false),
                    TipoAlerta = table.Column<int>(type: "int", nullable: false),
                    Severidade = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NivelConfianca = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DataGeracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataResolucao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasAmbientais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertasAmbientais_AreasMonitoradas_AreaMonitoradaId",
                        column: x => x.AreaMonitoradaId,
                        principalTable: "AreasMonitoradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndicadoresAmbientais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaMonitoradaId = table.Column<int>(type: "int", nullable: false),
                    TipoIndicador = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnidadeMedida = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DataLeitura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fonte = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicadoresAmbientais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicadoresAmbientais_AreasMonitoradas_AreaMonitoradaId",
                        column: x => x.AreaMonitoradaId,
                        principalTable: "AreasMonitoradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecomendacoesSustentaveis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertaAmbientalId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    BeneficioEsperado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DataGeracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAplicacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecomendacoesSustentaveis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecomendacoesSustentaveis_AlertasAmbientais_AlertaAmbientalId",
                        column: x => x.AlertaAmbientalId,
                        principalTable: "AlertasAmbientais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertasAmbientais_AreaMonitoradaId",
                table: "AlertasAmbientais",
                column: "AreaMonitoradaId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicadoresAmbientais_AreaMonitoradaId",
                table: "IndicadoresAmbientais",
                column: "AreaMonitoradaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecomendacoesSustentaveis_AlertaAmbientalId",
                table: "RecomendacoesSustentaveis",
                column: "AlertaAmbientalId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndicadoresAmbientais");

            migrationBuilder.DropTable(
                name: "RecomendacoesSustentaveis");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "AlertasAmbientais");

            migrationBuilder.DropTable(
                name: "AreasMonitoradas");
        }
    }
}
