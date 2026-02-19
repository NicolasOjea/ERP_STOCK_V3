using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206220000_AddCajaTables")]
public partial class AddCajaTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "cajas",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_cajas", x => x.Id);
                table.ForeignKey(
                    name: "FK_cajas_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_cajas_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "caja_sesiones",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                CajaId = table.Column<Guid>(type: "uuid", nullable: false),
                SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                MontoInicial = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                MontoCierre = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                DiferenciaTotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                MotivoDiferencia = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                ArqueoJson = table.Column<string>(type: "jsonb", nullable: true),
                AperturaAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CierreAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Estado = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_caja_sesiones", x => x.Id);
                table.ForeignKey(
                    name: "FK_caja_sesiones_cajas_CajaId",
                    column: x => x.CajaId,
                    principalTable: "cajas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_caja_sesiones_sucursales_SucursalId",
                    column: x => x.SucursalId,
                    principalTable: "sucursales",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_caja_sesiones_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "caja_movimientos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                CajaSesionId = table.Column<Guid>(type: "uuid", nullable: false),
                Tipo = table.Column<int>(type: "integer", nullable: false),
                MedioPago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Monto = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_caja_movimientos", x => x.Id);
                table.ForeignKey(
                    name: "FK_caja_movimientos_caja_sesiones_CajaSesionId",
                    column: x => x.CajaSesionId,
                    principalTable: "caja_sesiones",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_caja_movimientos_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_caja_movimientos_CajaSesionId",
            table: "caja_movimientos",
            column: "CajaSesionId");

        migrationBuilder.CreateIndex(
            name: "IX_caja_movimientos_Fecha",
            table: "caja_movimientos",
            column: "Fecha");

        migrationBuilder.CreateIndex(
            name: "IX_caja_movimientos_MedioPago",
            table: "caja_movimientos",
            column: "MedioPago");

        migrationBuilder.CreateIndex(
            name: "IX_caja_movimientos_TenantId",
            table: "caja_movimientos",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_caja_sesiones_AperturaAt",
            table: "caja_sesiones",
            column: "AperturaAt");

        migrationBuilder.CreateIndex(
            name: "IX_caja_sesiones_CajaId",
            table: "caja_sesiones",
            column: "CajaId");

        migrationBuilder.CreateIndex(
            name: "IX_caja_sesiones_CajaId_Estado",
            table: "caja_sesiones",
            columns: new[] { "CajaId", "Estado" },
            unique: true,
            filter: "\"Estado\" = 0");

        migrationBuilder.CreateIndex(
            name: "IX_caja_sesiones_SucursalId",
            table: "caja_sesiones",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_caja_sesiones_TenantId",
            table: "caja_sesiones",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_cajas_SucursalId",
            table: "cajas",
            column: "SucursalId");

        migrationBuilder.CreateIndex(
            name: "IX_cajas_TenantId",
            table: "cajas",
            column: "TenantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "caja_movimientos");

        migrationBuilder.DropTable(
            name: "caja_sesiones");

        migrationBuilder.DropTable(
            name: "cajas");
    }
}

