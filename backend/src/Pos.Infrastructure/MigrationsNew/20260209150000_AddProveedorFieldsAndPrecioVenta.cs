using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.MigrationsNew;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260209150000_AddProveedorFieldsAndPrecioVenta")]
public partial class AddProveedorFieldsAndPrecioVenta : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Telefono",
            table: "proveedores",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Cuit",
            table: "proveedores",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Direccion",
            table: "proveedores",
            type: "character varying(250)",
            maxLength: 250,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "PrecioVenta",
            table: "productos",
            type: "numeric(18,4)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.Sql("UPDATE productos SET \"PrecioVenta\" = \"PrecioBase\" WHERE \"PrecioVenta\" = 0");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Telefono",
            table: "proveedores");

        migrationBuilder.DropColumn(
            name: "Cuit",
            table: "proveedores");

        migrationBuilder.DropColumn(
            name: "Direccion",
            table: "proveedores");

        migrationBuilder.DropColumn(
            name: "PrecioVenta",
            table: "productos");
    }
}
