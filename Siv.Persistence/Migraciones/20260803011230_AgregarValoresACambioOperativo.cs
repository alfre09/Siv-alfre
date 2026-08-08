using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Siv.Persistence.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarValoresACambioOperativo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValorAnterior",
                table: "CambiosOperativos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValorNuevo",
                table: "CambiosOperativos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorAnterior",
                table: "CambiosOperativos");

            migrationBuilder.DropColumn(
                name: "ValorNuevo",
                table: "CambiosOperativos");
        }
    }
}
