using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206210000_AddStockMovimientos")]
public partial class AddStockMovimientos : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "stock_movimientos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                Tipo = table.Column<int>(type: "integer", nullable: false),
                Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_stock_movimientos", x => x.Id);
                table.ForeignKey(
                    name: "FK_stock_movimientos_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_stock_movimientos_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "stock_movimiento_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                MovimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                EsIngreso = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_stock_movimiento_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_stock_movimiento_items_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_stock_movimiento_items_stock_movimientos_MovimientoId",
                    column: x => x.MovimientoId,
                    principalTable: "stock_movimientos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_stock_movimiento_items_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_stock_movimiento_items_MovimientoId",
            table: "stock_movimiento_items",
            column: "MovimientoId");

        migrationBuilder.CreateIndex(
            name: "IX_stock_movimiento_items_ProductoId",
            table: "stock_movimiento_items",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_stock_movimiento_items_TenantId",
            table: "stock_movimiento_items",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_stock_movimientos_Fecha",
            table: "stock_movimientos",
            column: "Fecha");

        migrationBuilder.CreateIndex(
            name: "IX_stock_movimientos_SucursalId",
            table: "stock_movimientos",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_stock_movimientos_TenantId",
            table: "stock_movimientos",
            column: "TenantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "stock_movimiento_items");

        migrationBuilder.DropTable(
            name: "stock_movimientos");
    }
}

