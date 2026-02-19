using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260209193000_AddAdminPermissions")]
public partial class AddAdminPermissions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var seedTimestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var tenantId = new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01");
        var adminRoleId = new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03");

        migrationBuilder.InsertData(
            table: "rol_permisos",
            columns: new[] { "Id", "TenantId", "RoleId", "PermissionId", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "uuid", "uuid", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[,]
            {
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c19"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1a"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1b"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a003"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1c"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a004"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1d"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a005"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1e"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a006"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1f"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a007"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c20"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a008"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c21"),
                    tenantId,
                    adminRoleId,
                    new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a009"),
                    seedTimestamp,
                    seedTimestamp,
                    null
                }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        var ids = new[]
        {
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c19",
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1a",
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1b",
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1c",
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1d",
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1e",
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1f",
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c20",
            "a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c21"
        };

        foreach (var id in ids)
        {
            migrationBuilder.DeleteData(
                table: "rol_permisos",
                keyColumn: "Id",
                keyColumnType: "uuid",
                keyValue: new Guid(id));
        }
    }
}
