using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206224000_AddVentas")]
public partial class AddVentas : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ventas",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: true),
                ListaPrecio = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Estado = table.Column<int>(type: "integer", nullable: false),
                TotalNeto = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                TotalPagos = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ventas", x => x.Id);
                table.ForeignKey(
                    name: "FK_ventas_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ventas_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ventas_usuarios_UserId",
                    column: x => x.UserId,
                    principalTable: "usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "venta_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_venta_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_venta_items_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_venta_items_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_venta_items_ventas_VentaId",
                    column: x => x.VentaId,
                    principalTable: "ventas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "venta_pagos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                MedioPago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Monto = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_venta_pagos", x => x.Id);
                table.ForeignKey(
                    name: "FK_venta_pagos_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_venta_pagos_ventas_VentaId",
                    column: x => x.VentaId,
                    principalTable: "ventas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_venta_items_ProductoId",
            table: "venta_items",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_venta_items_TenantId",
            table: "venta_items",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_venta_items_VentaId",
            table: "venta_items",
            column: "VentaId");

        migrationBuilder.CreateIndex(
            name: "IX_venta_items_VentaId_ProductoId",
            table: "venta_items",
            columns: new[] { "VentaId", "ProductoId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_venta_pagos_MedioPago",
            table: "venta_pagos",
            column: "MedioPago");

        migrationBuilder.CreateIndex(
            name: "IX_venta_pagos_TenantId",
            table: "venta_pagos",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_venta_pagos_VentaId",
            table: "venta_pagos",
            column: "VentaId");

        migrationBuilder.CreateIndex(
            name: "IX_ventas_CreatedAt",
            table: "ventas",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_ventas_Estado",
            table: "ventas",
            column: "Estado");

        migrationBuilder.CreateIndex(
            name: "IX_ventas_SucursalId",
            table: "ventas",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_ventas_TenantId",
            table: "ventas",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_ventas_UserId",
            table: "ventas",
            column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "venta_pagos");

        migrationBuilder.DropTable(
            name: "venta_items");

        migrationBuilder.DropTable(
            name: "ventas");
    }
}

