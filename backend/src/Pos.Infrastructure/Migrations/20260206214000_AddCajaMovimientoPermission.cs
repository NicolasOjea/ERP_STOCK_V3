using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206214000_AddCajaMovimientoPermission")]
public partial class AddCajaMovimientoPermission : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var seedTimestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var tenantId = new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01");
        var permissionId = new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a016");

        migrationBuilder.InsertData(
            table: "permisos",
            columns: new[] { "Id", "TenantId", "Code", "Description", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "character varying(120)", "character varying(300)", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[]
            {
                permissionId,
                tenantId,
                "CAJA_MOVIMIENTO",
                "Movimientos de caja",
                seedTimestamp,
                seedTimestamp,
                null
            });

        migrationBuilder.InsertData(
            table: "rol_permisos",
            columns: new[] { "Id", "TenantId", "RoleId", "PermissionId", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "uuid", "uuid", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[,]
            {
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c17"),
                    tenantId,
                    new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"),
                    permissionId,
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c18"),
                    tenantId,
                    new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"),
                    permissionId,
                    seedTimestamp,
                    seedTimestamp,
                    null
                }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "rol_permisos",
            keyColumn: "Id",
            keyColumnType: "uuid",
            keyValue: new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c17"));

        migrationBuilder.DeleteData(
            table: "rol_permisos",
            keyColumn: "Id",
            keyColumnType: "uuid",
            keyValue: new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c18"));

        migrationBuilder.DeleteData(
            table: "permisos",
            keyColumn: "Id",
            keyColumnType: "uuid",
            keyValue: new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a016"));
    }
}

