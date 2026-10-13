using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TerraPulse.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsuarioAdministrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Ativo", "DataCadastro", "Email", "Nome", "Perfil", "SenhaHash" },
                values: new object[] { 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@terrapulse.ai", "Administrador TerraPulse", 1, "46984DD149A622441431BED3024ECC8036C9A2E970EF959BDCEAFCFF9A7CBF9C" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
