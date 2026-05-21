using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlTerritorial.Infrastructure.Migrations
{
    public partial class UpgradeUsuariosToSystemUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Dni_TenantId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Usuarios",
                newName: "Username");

            // ELIMINAR ROLE STRING VIEJO
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Usuarios");

            // NUEVO SYSTEMROLE INT
            migrationBuilder.AddColumn<int>(
                name: "SystemRole",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonaId",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PersonaId",
                table: "Usuarios",
                column: "PersonaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Personas_PersonaId",
                table: "Usuarios",
                column: "PersonaId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Personas_PersonaId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PersonaId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "SystemRole",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PersonaId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Usuarios",
                newName: "TenantId");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Usuarios",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "User");

            migrationBuilder.AddColumn<string>(
                name: "Dni",
                table: "Usuarios",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Dni_TenantId",
                table: "Usuarios",
                columns: new[] { "Dni", "TenantId" },
                unique: true);
        }
    }
}