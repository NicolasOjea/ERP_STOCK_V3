using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206213500_AddProveedores")]
public partial class AddProveedores : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "proveedores",
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
                table.PrimaryKey("PK_proveedores", x => x.Id);
                table.ForeignKey(
                    name: "FK_proveedores_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.AddColumn<Guid>(
            name: "ProveedorId",
            table: "productos",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_proveedores_TenantId",
            table: "proveedores",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_productos_ProveedorId",
            table: "productos",
            column: "ProveedorId");

        migrationBuilder.AddForeignKey(
            name: "FK_productos_proveedores_ProveedorId",
            table: "productos",
            column: "ProveedorId",
            principalTable: "proveedores",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_productos_proveedores_ProveedorId",
            table: "productos");

        migrationBuilder.DropIndex(
            name: "IX_productos_ProveedorId",
            table: "productos");

        migrationBuilder.DropColumn(
            name: "ProveedorId",
            table: "productos");

        migrationBuilder.DropTable(
            name: "proveedores");
    }
}

