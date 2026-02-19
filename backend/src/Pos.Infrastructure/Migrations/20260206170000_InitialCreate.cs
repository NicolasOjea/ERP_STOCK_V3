using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pos.Infrastructure.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(Pos.Infrastructure.Persistence.PosDbContext))]
[Migration("20260206170000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "tenants",
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
                table.PrimaryKey("PK_tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "permisos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_permisos", x => x.Id);
                table.ForeignKey(
                    name: "FK_permisos_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "roles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_roles", x => x.Id);
                table.ForeignKey(
                    name: "FK_roles_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "sucursales",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_sucursales", x => x.Id);
                table.ForeignKey(
                    name: "FK_sucursales_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "usuarios",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_usuarios", x => x.Id);
                table.ForeignKey(
                    name: "FK_usuarios_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "audit_logs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: true),
                Action = table.Column<int>(type: "integer", nullable: false),
                EntityName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                EntityId = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Detail = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_audit_logs", x => x.Id);
                table.ForeignKey(
                    name: "FK_audit_logs_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "rol_permisos",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_rol_permisos", x => x.Id);
                table.ForeignKey(
                    name: "FK_rol_permisos_permisos_PermissionId",
                    column: x => x.PermissionId,
                    principalTable: "permisos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_rol_permisos_roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_rol_permisos_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "usuario_roles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_usuario_roles", x => x.Id);
                table.ForeignKey(
                    name: "FK_usuario_roles_roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_usuario_roles_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_usuario_roles_usuarios_UserId",
                    column: x => x.UserId,
                    principalTable: "usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_audit_logs_TenantId",
            table: "audit_logs",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_permisos_TenantId",
            table: "permisos",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_permisos_TenantId_Code",
            table: "permisos",
            columns: new[] { "TenantId", "Code" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_rol_permisos_PermissionId",
            table: "rol_permisos",
            column: "PermissionId");

        migrationBuilder.CreateIndex(
            name: "IX_rol_permisos_RoleId",
            table: "rol_permisos",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_rol_permisos_TenantId",
            table: "rol_permisos",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_rol_permisos_TenantId_RoleId_PermissionId",
            table: "rol_permisos",
            columns: new[] { "TenantId", "RoleId", "PermissionId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_roles_TenantId",
            table: "roles",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_roles_TenantId_Name",
            table: "roles",
            columns: new[] { "TenantId", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_sucursales_TenantId",
            table: "sucursales",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_tenants_TenantId",
            table: "tenants",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_usuario_roles_RoleId",
            table: "usuario_roles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "IX_usuario_roles_TenantId",
            table: "usuario_roles",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_usuario_roles_TenantId_UserId_RoleId",
            table: "usuario_roles",
            columns: new[] { "TenantId", "UserId", "RoleId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_usuario_roles_UserId",
            table: "usuario_roles",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_usuarios_TenantId",
            table: "usuarios",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_usuarios_TenantId_Username",
            table: "usuarios",
            columns: new[] { "TenantId", "Username" },
            unique: true);

        var seedTimestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

        migrationBuilder.InsertData(
            table: "tenants",
            columns: new[] { "Id", "TenantId", "Name", "IsActive", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "character varying(200)", "boolean", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[]
            {
                new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"),
                new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"),
                "Demo",
                true,
                seedTimestamp,
                seedTimestamp,
                null
            });

        migrationBuilder.InsertData(
            table: "sucursales",
            columns: new[] { "Id", "TenantId", "Name", "Code", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "character varying(200)", "character varying(50)", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[]
            {
                new Guid("3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1"),
                new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"),
                "Central",
                "CENTRAL",
                seedTimestamp,
                seedTimestamp,
                null
            });

        migrationBuilder.InsertData(
            table: "usuarios",
            columns: new[] { "Id", "TenantId", "Username", "PasswordHash", "IsActive", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "character varying(100)", "character varying(512)", "boolean", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[]
            {
                new Guid("c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b"),
                new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"),
                "admin",
                "sha256:3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121",
                true,
                seedTimestamp,
                seedTimestamp,
                null
            });

        migrationBuilder.InsertData(
            table: "roles",
            columns: new[] { "Id", "TenantId", "Name", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "character varying(100)", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[,]
            {
                {
                    new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"),
                    new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"),
                    "CAJERO",
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"),
                    new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"),
                    "ENCARGADO",
                    seedTimestamp,
                    seedTimestamp,
                    null
                },
                {
                    new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"),
                    new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"),
                    "ADMIN",
                    seedTimestamp,
                    seedTimestamp,
                    null
                }
            });

        migrationBuilder.InsertData(
            table: "permisos",
            columns: new[] { "Id", "TenantId", "Code", "Description", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "character varying(120)", "character varying(300)", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[,]
            {
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "VENTA_CREAR", "Crear venta", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "VENTA_CONFIRMAR", "Confirmar venta", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a003"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "VENTA_ANULAR", "Anular venta", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a004"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "DEVOLUCION_REGISTRAR", "Registrar devolucion", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a005"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "CAJA_ABRIR", "Abrir caja", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a006"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "CAJA_CERRAR", "Cerrar caja", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a007"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "STOCK_AJUSTAR", "Ajustar stock", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a008"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "PRODUCTO_VER", "Ver productos", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a009"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "PRODUCTO_EDITAR", "Editar productos", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a010"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "USUARIO_ADMIN", "Administrar usuarios", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a011"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "REPORTES_VER", "Ver reportes", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a012"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "CLIENTE_GESTIONAR", "Gestionar clientes", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a013"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "PROVEEDOR_GESTIONAR", "Gestionar proveedores", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a014"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "COMPRAS_REGISTRAR", "Registrar compras", seedTimestamp, seedTimestamp, null },
                { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a015"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), "CONFIGURACION_VER", "Ver configuracion", seedTimestamp, seedTimestamp, null }
            });

        migrationBuilder.InsertData(
            table: "usuario_roles",
            columns: new[] { "Id", "TenantId", "UserId", "RoleId", "CreatedAt", "UpdatedAt", "DeletedAt" },
            columnTypes: new[] { "uuid", "uuid", "uuid", "uuid", "timestamp with time zone", "timestamp with time zone", "timestamp with time zone" },
            values: new object[]
            {
                new Guid("f19b3d0a-92f1-4b7d-9d3f-05b0f8f1a101"),
                new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"),
                new Guid("c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b"),
                new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"),
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
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c01"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c02"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a005"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c04"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a006"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c05"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c06"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c07"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a003"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c08"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a007"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c09"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a008"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c10"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a009"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c11"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a010"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c12"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a011"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c13"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a012"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c14"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a013"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c15"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a014"), seedTimestamp, seedTimestamp, null },
                { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c16"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a015"), seedTimestamp, seedTimestamp, null }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "rol_permisos");

        migrationBuilder.DropTable(
            name: "usuario_roles");

        migrationBuilder.DropTable(
            name: "audit_logs");

        migrationBuilder.DropTable(
            name: "permisos");

        migrationBuilder.DropTable(
            name: "roles");

        migrationBuilder.DropTable(
            name: "usuarios");

        migrationBuilder.DropTable(
            name: "sucursales");

        migrationBuilder.DropTable(
            name: "tenants");
    }
}


