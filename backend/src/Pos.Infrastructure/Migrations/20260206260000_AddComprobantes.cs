using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206260000_AddComprobantes")]
public partial class AddComprobantes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "comprobantes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: true),
                Estado = table.Column<int>(type: "integer", nullable: false),
                Total = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                Numero = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                FiscalProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                FiscalPayload = table.Column<string>(type: "text", nullable: true),
                EmitidoAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_comprobantes", x => x.Id);
                table.ForeignKey(
                    name: "FK_comprobantes_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_comprobantes_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_comprobantes_usuarios_UserId",
                    column: x => x.UserId,
                    principalTable: "usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_comprobantes_ventas_VentaId",
                    column: x => x.VentaId,
                    principalTable: "ventas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_comprobantes_CreatedAt",
            table: "comprobantes",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_comprobantes_Estado",
            table: "comprobantes",
            column: "Estado");

        migrationBuilder.CreateIndex(
            name: "IX_comprobantes_SucursalId",
            table: "comprobantes",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_comprobantes_TenantId",
            table: "comprobantes",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_comprobantes_VentaId",
            table: "comprobantes",
            column: "VentaId");

        migrationBuilder.CreateIndex(
            name: "IX_comprobantes_TenantId_VentaId",
            table: "comprobantes",
            columns: new[] { "TenantId", "VentaId" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "comprobantes");
    }
}

