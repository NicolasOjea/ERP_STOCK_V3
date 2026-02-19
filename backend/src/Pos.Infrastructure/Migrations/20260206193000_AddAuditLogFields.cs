using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206193000_AddAuditLogFields")]
public partial class AddAuditLogFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Detail",
            table: "audit_logs");

        migrationBuilder.AddColumn<Guid>(
            name: "SucursalId",
            table: "audit_logs",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<string>(
            name: "BeforeJson",
            table: "audit_logs",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AfterJson",
            table: "audit_logs",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "MetadataJson",
            table: "audit_logs",
            type: "text",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_audit_logs_SucursalId",
            table: "audit_logs",
            column: "SucursalId");

        migrationBuilder.AddForeignKey(
            name: "FK_audit_logs_sucursales_SucursalId",
            table: "audit_logs",
            column: "SucursalId",
            principalTable: "sucursales",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_audit_logs_sucursales_SucursalId",
            table: "audit_logs");

        migrationBuilder.DropIndex(
            name: "IX_audit_logs_SucursalId",
            table: "audit_logs");

        migrationBuilder.DropColumn(
            name: "AfterJson",
            table: "audit_logs");

        migrationBuilder.DropColumn(
            name: "BeforeJson",
            table: "audit_logs");

        migrationBuilder.DropColumn(
            name: "MetadataJson",
            table: "audit_logs");

        migrationBuilder.DropColumn(
            name: "SucursalId",
            table: "audit_logs");

        migrationBuilder.AddColumn<string>(
            name: "Detail",
            table: "audit_logs",
            type: "character varying(2000)",
            maxLength: 2000,
            nullable: true);
    }
}

