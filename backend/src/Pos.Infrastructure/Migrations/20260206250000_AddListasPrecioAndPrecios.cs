using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206250000_AddListasPrecioAndPrecios")]
public partial class AddListasPrecioAndPrecios : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "PrecioBase",
            table: "productos",
            type: "numeric(18,4)",
            nullable: false,
            defaultValue: 1m);

        migrationBuilder.AddColumn<decimal>(
            name: "PrecioUnitario",
            table: "venta_items",
            type: "numeric(18,4)",
            nullable: false,
            defaultValue: 1m);

        migrationBuilder.CreateTable(
            name: "listas_precio",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_listas_precio", x => x.Id);
                table.ForeignKey(
                    name: "FK_listas_precio_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "lista_precio_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                ListaPrecioId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                Precio = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_lista_precio_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_lista_precio_items_listas_precio_ListaPrecioId",
                    column: x => x.ListaPrecioId,
                    principalTable: "listas_precio",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_lista_precio_items_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_lista_precio_items_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_listas_precio_TenantId",
            table: "listas_precio",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_listas_precio_TenantId_Nombre",
            table: "listas_precio",
            columns: new[] { "TenantId", "Nombre" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_lista_precio_items_ListaPrecioId",
            table: "lista_precio_items",
            column: "ListaPrecioId");

        migrationBuilder.CreateIndex(
            name: "IX_lista_precio_items_ProductoId",
            table: "lista_precio_items",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_lista_precio_items_TenantId",
            table: "lista_precio_items",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_lista_precio_items_TenantId_ListaPrecioId_ProductoId",
            table: "lista_precio_items",
            columns: new[] { "TenantId", "ListaPrecioId", "ProductoId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "lista_precio_items");

        migrationBuilder.DropTable(
            name: "listas_precio");

        migrationBuilder.DropColumn(
            name: "PrecioBase",
            table: "productos");

        migrationBuilder.DropColumn(
            name: "PrecioUnitario",
            table: "venta_items");
    }
}

