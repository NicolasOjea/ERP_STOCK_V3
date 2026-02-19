using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206203000_AddStockConfigAndSaldos")]
public partial class AddStockConfigAndSaldos : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "producto_stock_config",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                StockMinimo = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                ToleranciaPct = table.Column<decimal>(type: "numeric(6,2)", nullable: false, defaultValue: 25m),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_producto_stock_config", x => x.Id);
                table.ForeignKey(
                    name: "FK_producto_stock_config_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_producto_stock_config_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_producto_stock_config_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "stock_saldos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                CantidadActual = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_stock_saldos", x => x.Id);
                table.ForeignKey(
                    name: "FK_stock_saldos_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_stock_saldos_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_stock_saldos_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_producto_stock_config_ProductoId",
            table: "producto_stock_config",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_producto_stock_config_SucursalId",
            table: "producto_stock_config",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_producto_stock_config_TenantId",
            table: "producto_stock_config",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_producto_stock_config_TenantId_ProductoId_SucursalId",
            table: "producto_stock_config",
            columns: new[] { "TenantId", "ProductoId", "SucursalId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_stock_saldos_ProductoId",
            table: "stock_saldos",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_stock_saldos_SucursalId",
            table: "stock_saldos",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_stock_saldos_TenantId",
            table: "stock_saldos",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_stock_saldos_TenantId_ProductoId_SucursalId",
            table: "stock_saldos",
            columns: new[] { "TenantId", "ProductoId", "SucursalId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "producto_stock_config");

        migrationBuilder.DropTable(
            name: "stock_saldos");
    }
}

