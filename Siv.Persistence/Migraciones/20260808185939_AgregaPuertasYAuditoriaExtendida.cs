using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Siv.Persistence.Migraciones
{
    /// <inheritdoc />
    public partial class AgregaPuertasYAuditoriaExtendida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistroId",
                table: "Auditorias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "Auditorias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ValorAnterior",
                table: "Auditorias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValorNuevo",
                table: "Auditorias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Puertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AeropuertoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Puertas_Aeropuertos_AeropuertoId",
                        column: x => x.AeropuertoId,
                        principalTable: "Aeropuertos",
                        principalColumn: "AeropuertoId",
                        onDelete: ReferentialAction.Restrict);
                });



            migrationBuilder.CreateIndex(
                name: "IX_Puertas_AeropuertoId",
                table: "Puertas",
                column: "AeropuertoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Puertas");

            migrationBuilder.DropColumn(
                name: "RegistroId",
                table: "Auditorias");

            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "Auditorias");

            migrationBuilder.DropColumn(
                name: "ValorAnterior",
                table: "Auditorias");

            migrationBuilder.DropColumn(
                name: "ValorNuevo",
                table: "Auditorias");
        }
    }
}
