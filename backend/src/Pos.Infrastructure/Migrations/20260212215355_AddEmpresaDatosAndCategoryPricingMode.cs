using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaDatosAndCategoryPricingMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'ventas'
                          AND column_name = 'Numero'
                    ) THEN
                        ALTER TABLE ventas RENAME COLUMN "Numero" TO numero;
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM pg_indexes
                        WHERE schemaname = 'public'
                          AND tablename = 'ventas'
                          AND indexname = 'IX_ventas_TenantId_Numero'
                    ) THEN
                        ALTER INDEX "IX_ventas_TenantId_Numero" RENAME TO "IX_ventas_TenantId_numero";
                    END IF;
                END $$;
                """);

            migrationBuilder.AddColumn<string>(
                name: "Cuit",
                table: "proveedores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "proveedores",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "proveedores",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MargenGananciaPct",
                table: "productos",
                type: "numeric(6,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioVenta",
                table: "productos",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 1m);

            migrationBuilder.AddColumn<int>(
                name: "PricingMode",
                table: "productos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MargenGananciaPct",
                table: "categorias",
                type: "numeric(6,2)",
                nullable: false,
                defaultValue: 30m);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "cajas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "empresa_datos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Cuit = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Email = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Web = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empresa_datos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_empresa_datos_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cajas_TenantId_SucursalId_Numero",
                table: "cajas",
                columns: new[] { "TenantId", "SucursalId", "Numero" },
                unique: true,
                filter: "\"Numero\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_empresa_datos_TenantId",
                table: "empresa_datos",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "empresa_datos");

            migrationBuilder.DropIndex(
                name: "IX_cajas_TenantId_SucursalId_Numero",
                table: "cajas");

            migrationBuilder.DropColumn(
                name: "Cuit",
                table: "proveedores");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "proveedores");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "proveedores");

            migrationBuilder.DropColumn(
                name: "MargenGananciaPct",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "PrecioVenta",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "PricingMode",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "MargenGananciaPct",
                table: "categorias");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "cajas");

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_schema = 'public'
                          AND table_name = 'ventas'
                          AND column_name = 'numero'
                    ) THEN
                        ALTER TABLE ventas RENAME COLUMN numero TO "Numero";
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM pg_indexes
                        WHERE schemaname = 'public'
                          AND tablename = 'ventas'
                          AND indexname = 'IX_ventas_TenantId_numero'
                    ) THEN
                        ALTER INDEX "IX_ventas_TenantId_numero" RENAME TO "IX_ventas_TenantId_Numero";
                    END IF;
                END $$;
                """);
        }
    }
}
