using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Siv.Persistence.Migraciones
{
    /// <inheritdoc />
    public partial class AddCambioOperativoToNotificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CambioOperativoId",
                table: "Notificaciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_CambioOperativoId",
                table: "Notificaciones",
                column: "CambioOperativoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_CambiosOperativos_CambioOperativoId",
                table: "Notificaciones",
                column: "CambioOperativoId",
                principalTable: "CambiosOperativos",
                principalColumn: "CambioOperativoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_CambiosOperativos_CambioOperativoId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_CambioOperativoId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "CambioOperativoId",
                table: "Notificaciones");
        }
    }
}
