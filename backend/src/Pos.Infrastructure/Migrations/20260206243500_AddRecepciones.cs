using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206243500_AddRecepciones")]
public partial class AddRecepciones : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "recepciones",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                PreRecepcionId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_recepciones", x => x.Id);
                table.ForeignKey(
                    name: "FK_recepciones_pre_recepciones_PreRecepcionId",
                    column: x => x.PreRecepcionId,
                    principalTable: "pre_recepciones",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_recepciones_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_recepciones_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "recepcion_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                RecepcionId = table.Column<Guid>(type: "uuid", nullable: false),
                PreRecepcionItemId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
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
                table.PrimaryKey("PK_recepcion_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_recepcion_items_pre_recepcion_items_PreRecepcionItemId",
                    column: x => x.PreRecepcionItemId,
                    principalTable: "pre_recepcion_items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_recepcion_items_productos_ProductoId",
                    column: x => x.ProductoId,
                    principalTable: "productos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_recepcion_items_recepciones_RecepcionId",
                    column: x => x.RecepcionId,
                    principalTable: "recepciones",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_recepcion_items_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_recepcion_items_PreRecepcionItemId",
            table: "recepcion_items",
            column: "PreRecepcionItemId");

        migrationBuilder.CreateIndex(
            name: "IX_recepcion_items_ProductoId",
            table: "recepcion_items",
            column: "ProductoId");

        migrationBuilder.CreateIndex(
            name: "IX_recepcion_items_RecepcionId",
            table: "recepcion_items",
            column: "RecepcionId");

        migrationBuilder.CreateIndex(
            name: "IX_recepcion_items_TenantId",
            table: "recepcion_items",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_recepcion_items_RecepcionId_PreRecepcionItemId",
            table: "recepcion_items",
            columns: new[] { "RecepcionId", "PreRecepcionItemId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_recepciones_CreatedAt",
            table: "recepciones",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_recepciones_PreRecepcionId",
            table: "recepciones",
            column: "PreRecepcionId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_recepciones_SucursalId",
            table: "recepciones",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_recepciones_TenantId",
            table: "recepciones",
            column: "TenantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "recepcion_items");

        migrationBuilder.DropTable(
            name: "recepciones");
    }
}

