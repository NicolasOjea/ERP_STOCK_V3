using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206230000_AddProductoProveedor")]
public partial class AddProductoProveedor : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "producto_proveedor",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                ProveedorId = table.Column<Guid>(type: "uuid", nullable: false),
                EsPrincipal = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_producto_proveedor", x => x.Id);
                table.ForeignKey(
                    name: "FK_producto_proveedor_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_producto_proveedor_proveedores_ProveedorId",
                    column: x => x.ProveedorId,
                    principalTable: "proveedores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_producto_proveedor_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_producto_proveedor_ProductoId",
            table: "producto_proveedor",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_producto_proveedor_ProveedorId",
            table: "producto_proveedor",
            column: "ProveedorId");

        migrationBuilder.CreateIndex(
            name: "IX_producto_proveedor_TenantId",
            table: "producto_proveedor",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_producto_proveedor_TenantId_ProductoId",
            table: "producto_proveedor",
            columns: new[] { "TenantId", "ProductoId" },
            unique: true,
            filter: "\"EsPrincipal\" = true");

        migrationBuilder.CreateIndex(
            name: "IX_producto_proveedor_TenantId_ProductoId_ProveedorId",
            table: "producto_proveedor",
            columns: new[] { "TenantId", "ProductoId", "ProveedorId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "producto_proveedor");
    }
}

