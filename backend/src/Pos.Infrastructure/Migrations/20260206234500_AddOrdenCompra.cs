using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206234500_AddOrdenCompra")]
public partial class AddOrdenCompra : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ordenes_compra",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                ProveedorId = table.Column<Guid>(type: "uuid", nullable: true),
                Estado = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ordenes_compra", x => x.Id);
                table.ForeignKey(
                    name: "FK_ordenes_compra_proveedores_ProveedorId",
                    column: x => x.ProveedorId,
                    principalTable: "proveedores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ordenes_compra_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ordenes_compra_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "orden_compra_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                OrdenCompraId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_orden_compra_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_orden_compra_items_ordenes_compra_OrdenCompraId",
                    column: x => x.OrdenCompraId,
                    principalTable: "ordenes_compra",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_orden_compra_items_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_orden_compra_items_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_orden_compra_items_OrdenCompraId",
            table: "orden_compra_items",
            column: "OrdenCompraId");

        migrationBuilder.CreateIndex(
            name: "IX_orden_compra_items_ProductoId",
            table: "orden_compra_items",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_orden_compra_items_TenantId",
            table: "orden_compra_items",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_orden_compra_items_OrdenCompraId_ProductoId",
            table: "orden_compra_items",
            columns: new[] { "OrdenCompraId", "ProductoId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ordenes_compra_CreatedAt",
            table: "ordenes_compra",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_ordenes_compra_Estado",
            table: "ordenes_compra",
            column: "Estado");

        migrationBuilder.CreateIndex(
            name: "IX_ordenes_compra_ProveedorId",
            table: "ordenes_compra",
            column: "ProveedorId");

        migrationBuilder.CreateIndex(
            name: "IX_ordenes_compra_SucursalId",
            table: "ordenes_compra",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_ordenes_compra_TenantId",
            table: "ordenes_compra",
            column: "TenantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "orden_compra_items");

        migrationBuilder.DropTable(
            name: "ordenes_compra");
    }
}

