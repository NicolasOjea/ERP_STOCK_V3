using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206241000_AddDocumentosCompraPreRecepcion")]
public partial class AddDocumentosCompraPreRecepcion : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "documentos_compra",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                ProveedorId = table.Column<Guid>(type: "uuid", nullable: true),
                Numero = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Fecha = table.Column<DateTime>(type: "date", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_documentos_compra", x => x.Id);
                table.ForeignKey(
                    name: "FK_documentos_compra_proveedores_ProveedorId",
                    column: x => x.ProveedorId,
                    principalTable: "proveedores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_documentos_compra_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_documentos_compra_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "documento_compra_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                DocumentoCompraId = table.Column<Guid>(type: "uuid", nullable: false),
                Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                CostoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_documento_compra_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_documento_compra_items_documentos_compra_DocumentoCompraId",
                    column: x => x.DocumentoCompraId,
                    principalTable: "documentos_compra",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_documento_compra_items_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "pre_recepciones",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                DocumentoCompraId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_pre_recepciones", x => x.Id);
                table.ForeignKey(
                    name: "FK_pre_recepciones_documentos_compra_DocumentoCompraId",
                    column: x => x.DocumentoCompraId,
                    principalTable: "documentos_compra",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_pre_recepciones_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_pre_recepciones_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "pre_recepcion_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                PreRecepcionId = table.Column<Guid>(type: "uuid", nullable: false),
                DocumentoCompraItemId = table.Column<Guid>(type: "uuid", nullable: false),
                Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                CostoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: true),
                Estado = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_pre_recepcion_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_pre_recepcion_items_documento_compra_items_DocumentoCompraItemId",
                    column: x => x.DocumentoCompraItemId,
                    principalTable: "documento_compra_items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_pre_recepcion_items_pre_recepciones_PreRecepcionId",
                    column: x => x.PreRecepcionId,
                    principalTable: "pre_recepciones",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_pre_recepcion_items_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_pre_recepcion_items_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_documento_compra_items_Codigo",
            table: "documento_compra_items",
            column: "Codigo");

        migrationBuilder.CreateIndex(
            name: "IX_documento_compra_items_DocumentoCompraId",
            table: "documento_compra_items",
            column: "DocumentoCompraId");

        migrationBuilder.CreateIndex(
            name: "IX_documento_compra_items_TenantId",
            table: "documento_compra_items",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_documentos_compra_Fecha",
            table: "documentos_compra",
            column: "Fecha");

        migrationBuilder.CreateIndex(
            name: "IX_documentos_compra_Numero",
            table: "documentos_compra",
            column: "Numero");

        migrationBuilder.CreateIndex(
            name: "IX_documentos_compra_ProveedorId",
            table: "documentos_compra",
            column: "ProveedorId");

        migrationBuilder.CreateIndex(
            name: "IX_documentos_compra_SucursalId",
            table: "documentos_compra",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_documentos_compra_TenantId",
            table: "documentos_compra",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepcion_items_DocumentoCompraItemId",
            table: "pre_recepcion_items",
            column: "DocumentoCompraItemId");

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepcion_items_PreRecepcionId",
            table: "pre_recepcion_items",
            column: "PreRecepcionId");

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepcion_items_ProductoId",
            table: "pre_recepcion_items",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepcion_items_TenantId",
            table: "pre_recepcion_items",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepcion_items_PreRecepcionId_DocumentoCompraItemId",
            table: "pre_recepcion_items",
            columns: new[] { "PreRecepcionId", "DocumentoCompraItemId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepciones_CreatedAt",
            table: "pre_recepciones",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepciones_DocumentoCompraId",
            table: "pre_recepciones",
            column: "DocumentoCompraId");

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepciones_SucursalId",
            table: "pre_recepciones",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_pre_recepciones_TenantId",
            table: "pre_recepciones",
            column: "TenantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "pre_recepcion_items");

        migrationBuilder.DropTable(
            name: "pre_recepciones");

        migrationBuilder.DropTable(
            name: "documento_compra_items");

        migrationBuilder.DropTable(
            name: "documentos_compra");
    }
}

