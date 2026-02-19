using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.MigrationsNew;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260209170000_AddCajaNumero")]
public partial class AddCajaNumero : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Numero",
            table: "cajas",
            type: "character varying(50)",
            maxLength: 50,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_cajas_TenantId_SucursalId_Numero",
            table: "cajas",
            columns: new[] { "TenantId", "SucursalId", "Numero" },
            unique: true,
            filter: "\"Numero\" IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_cajas_TenantId_SucursalId_Numero",
            table: "cajas");

        migrationBuilder.DropColumn(
            name: "Numero",
            table: "cajas");
    }
}
