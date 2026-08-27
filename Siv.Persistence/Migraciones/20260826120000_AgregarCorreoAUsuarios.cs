using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Siv.Persistence.Migraciones;

public partial class AgregarCorreoAUsuarios : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Correo",
            table: "Usuarios",
            type: "nvarchar(320)",
            maxLength: 320,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Usuarios_Correo",
            table: "Usuarios",
            column: "Correo",
            unique: true,
            filter: "[Correo] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Usuarios_Correo", table: "Usuarios");
        migrationBuilder.DropColumn(name: "Correo", table: "Usuarios");
    }
}
