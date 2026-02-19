using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206200000_AddProductTables")]
public partial class AddProductTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "categorias",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_categorias", x => x.Id);
                table.ForeignKey(
                    name: "FK_categorias_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "marcas",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_marcas", x => x.Id);
                table.ForeignKey(
                    name: "FK_marcas_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "productos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                Sku = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                CategoriaId = table.Column<Guid>(type: "uuid", nullable: true),
                MarcaId = table.Column<Guid>(type: "uuid", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_productos", x => x.Id);
                table.ForeignKey(
                    name: "FK_productos_categorias_CategoriaId",
                    column: x => x.CategoriaId,
                    principalTable: "categorias",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_productos_marcas_MarcaId",
                    column: x => x.MarcaId,
                    principalTable: "marcas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_productos_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "producto_codigos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_producto_codigos", x => x.Id);
                table.ForeignKey(
                    name: "FK_producto_codigos_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_producto_codigos_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_categorias_TenantId",
            table: "categorias",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_marcas_TenantId",
            table: "marcas",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_productos_CategoriaId",
            table: "productos",
            column: "CategoriaId");

        migrationBuilder.CreateIndex(
            name: "IX_productos_MarcaId",
            table: "productos",
            column: "MarcaId");

        migrationBuilder.CreateIndex(
            name: "IX_productos_TenantId",
            table: "productos",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_productos_TenantId_Sku",
            table: "productos",
            columns: new[] { "TenantId", "Sku" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_producto_codigos_ProductoId",
            table: "producto_codigos",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_producto_codigos_TenantId",
            table: "producto_codigos",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_producto_codigos_TenantId_Codigo",
            table: "producto_codigos",
            columns: new[] { "TenantId", "Codigo" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "producto_codigos");

        migrationBuilder.DropTable(
            name: "productos");

        migrationBuilder.DropTable(
            name: "categorias");

        migrationBuilder.DropTable(
            name: "marcas");
    }
}

