using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206252000_AddDevoluciones")]
public partial class AddDevoluciones : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "devoluciones",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: true),
                Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Total = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_devoluciones", x => x.Id);
                table.ForeignKey(
                    name: "FK_devoluciones_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_devoluciones_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_devoluciones_usuarios_UserId",
                    column: x => x.UserId,
                    principalTable: "usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_devoluciones_ventas_VentaId",
                    column: x => x.VentaId,
                    principalTable: "ventas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "nota_credito_interna",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                DevolucionId = table.Column<Guid>(type: "uuid", nullable: false),
                Total = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_nota_credito_interna", x => x.Id);
                table.ForeignKey(
                    name: "FK_nota_credito_interna_devoluciones_DevolucionId",
                    column: x => x.DevolucionId,
                    principalTable: "devoluciones",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_nota_credito_interna_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_nota_credito_interna_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "devolucion_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                DevolucionId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                PrecioUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_devolucion_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_devolucion_items_devoluciones_DevolucionId",
                    column: x => x.DevolucionId,
                    principalTable: "devoluciones",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_devolucion_items_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_devolucion_items_DevolucionId",
            table: "devolucion_items",
            column: "DevolucionId");

        migrationBuilder.CreateIndex(
            name: "IX_devolucion_items_ProductoId",
            table: "devolucion_items",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_devolucion_items_TenantId",
            table: "devolucion_items",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_devolucion_items_DevolucionId_ProductoId",
            table: "devolucion_items",
            columns: new[] { "DevolucionId", "ProductoId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_devoluciones_CreatedAt",
            table: "devoluciones",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_devoluciones_SucursalId",
            table: "devoluciones",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_devoluciones_TenantId",
            table: "devoluciones",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_devoluciones_UserId",
            table: "devoluciones",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_devoluciones_VentaId",
            table: "devoluciones",
            column: "VentaId");

        migrationBuilder.CreateIndex(
            name: "IX_nota_credito_interna_DevolucionId",
            table: "nota_credito_interna",
            column: "DevolucionId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_nota_credito_interna_SucursalId",
            table: "nota_credito_interna",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_nota_credito_interna_TenantId",
            table: "nota_credito_interna",
            column: "TenantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "devolucion_items");

        migrationBuilder.DropTable(
            name: "nota_credito_interna");

        migrationBuilder.DropTable(
            name: "devoluciones");
    }
}

