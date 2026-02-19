using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pos.Infrastructure.MigrationsNew
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_categorias_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "listas_precio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_listas_precio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_listas_precio_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "marcas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marcas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_marcas_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permisos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "proveedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Sku = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uuid", nullable: true),
                    MarcaId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProveedorId = table.Column<Guid>(type: "uuid", nullable: true),
                    PrecioBase = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 1m),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_productos_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productos_marcas_MarcaId",
                        column: x => x.MarcaId,
                        principalTable: "marcas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productos_proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productos_tenants_TenantId",
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
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    EntityName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BeforeJson = table.Column<string>(type: "text", nullable: true),
                    AfterJson = table.Column<string>(type: "text", nullable: true),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_audit_logs_sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_audit_logs_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cajas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "documentos_compra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Numero = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "ordenes_compra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordenes_compra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ordenes_compra_proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_compra_sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordenes_compra_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_movimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_movimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stock_movimientos_sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_movimientos_tenants_TenantId",
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
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ListaPrecio = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    TotalNeto = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                    TotalPagos = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ventas_sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ventas_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ventas_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lista_precio_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListaPrecioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Precio = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lista_precio_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lista_precio_items_listas_precio_ListaPrecioId",
                        column: x => x.ListaPrecioId,
                        principalTable: "listas_precio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lista_precio_items_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lista_precio_items_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "producto_codigos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_codigos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_producto_codigos_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_producto_codigos_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "producto_proveedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_proveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_producto_proveedor_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_producto_proveedor_proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_producto_proveedor_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "producto_stock_config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    StockMinimo = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ToleranciaPct = table.Column<decimal>(type: "numeric(6,2)", nullable: false, defaultValue: 25m),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producto_stock_config", x => x.Id);
                    table.ForeignKey(
                        name: "FK_producto_stock_config_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_producto_stock_config_sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_producto_stock_config_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_saldos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    CantidadActual = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_saldos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stock_saldos_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_saldos_sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_saldos_tenants_TenantId",
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
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "documento_compra_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentoCompraId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentoCompraId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "orden_compra_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrdenCompraId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orden_compra_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orden_compra_items_ordenes_compra_OrdenCompraId",
                        column: x => x.OrdenCompraId,
                        principalTable: "ordenes_compra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orden_compra_items_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orden_compra_items_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_movimiento_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MovimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    EsIngreso = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_movimiento_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stock_movimiento_items_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stock_movimiento_items_stock_movimientos_MovimientoId",
                        column: x => x.MovimientoId,
                        principalTable: "stock_movimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stock_movimiento_items_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comprobantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                    Numero = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    FiscalProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FiscalPayload = table.Column<string>(type: "text", nullable: true),
                    EmitidoAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "devoluciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devoluciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devoluciones_sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_devoluciones_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_devoluciones_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_devoluciones_ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "venta_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venta_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_venta_items_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_venta_items_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_venta_items_ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "venta_pagos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedioPago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venta_pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_venta_pagos_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_venta_pagos_ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "caja_movimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CajaSesionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    MedioPago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "pre_recepcion_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PreRecepcionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentoCompraItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pre_recepcion_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pre_recepcion_items_documento_compra_items_DocumentoCompraI~",
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

            migrationBuilder.CreateTable(
                name: "recepciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreRecepcionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "devolucion_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DevolucionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devolucion_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devolucion_items_devoluciones_DevolucionId",
                        column: x => x.DevolucionId,
                        principalTable: "devoluciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_devolucion_items_productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nota_credito_interna",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    DevolucionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: false, defaultValue: 0m),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nota_credito_interna", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nota_credito_interna_devoluciones_DevolucionId",
                        column: x => x.DevolucionId,
                        principalTable: "devoluciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nota_credito_interna_sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nota_credito_interna_tenants_TenantId",
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
                    RecepcionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreRecepcionItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.InsertData(
                table: "tenants",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsActive", "Name", "TenantId", "UpdatedAt" },
                values: new object[] { new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, true, "Demo", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "Id", "Code", "CreatedAt", "DeletedAt", "Description", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001"), "VENTA_CREAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Crear venta", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002"), "VENTA_CONFIRMAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Confirmar venta", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a003"), "VENTA_ANULAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Anular venta", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a004"), "DEVOLUCION_REGISTRAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Registrar devolucion", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a005"), "CAJA_ABRIR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Abrir caja", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a006"), "CAJA_CERRAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Cerrar caja", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a007"), "STOCK_AJUSTAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Ajustar stock", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a008"), "PRODUCTO_VER", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Ver productos", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a009"), "PRODUCTO_EDITAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Editar productos", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a010"), "USUARIO_ADMIN", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Administrar usuarios", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a011"), "REPORTES_VER", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Ver reportes", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a012"), "CLIENTE_GESTIONAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Gestionar clientes", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a013"), "PROVEEDOR_GESTIONAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Gestionar proveedores", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a014"), "COMPRAS_REGISTRAR", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Registrar compras", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a015"), "CONFIGURACION_VER", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Ver configuracion", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a016"), "CAJA_MOVIMIENTO", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Movimientos de caja", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Name", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "CAJERO", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "ENCARGADO", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "ADMIN", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "sucursales",
                columns: new[] { "Id", "Code", "CreatedAt", "DeletedAt", "Name", "TenantId", "UpdatedAt" },
                values: new object[] { new Guid("3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1"), "CENTRAL", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, "Central", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsActive", "PasswordHash", "TenantId", "UpdatedAt", "Username" },
                values: new object[] { new Guid("c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, true, "sha256:3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121", new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin" });

            migrationBuilder.InsertData(
                table: "rol_permisos",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "PermissionId", "RoleId", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001"), new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c02"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002"), new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c03"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a005"), new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c04"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a006"), new Guid("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c05"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c06"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c07"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a003"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c08"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a007"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c09"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a008"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c10"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a009"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c11"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a010"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c12"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a011"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c13"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a012"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c14"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a013"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c15"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a014"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c16"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a015"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c17"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a016"), new Guid("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c18"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a016"), new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "usuario_roles",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "RoleId", "TenantId", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("f19b3d0a-92f1-4b7d-9d3f-05b0f8f1a101"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03"), new Guid("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b") });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_SucursalId",
                table: "audit_logs",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_TenantId",
                table: "audit_logs",
                column: "TenantId");

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
                filter: "\"Estado\" = 1");

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

            migrationBuilder.CreateIndex(
                name: "IX_categorias_TenantId",
                table: "categorias",
                column: "TenantId");

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
                name: "IX_comprobantes_TenantId_VentaId",
                table: "comprobantes",
                columns: new[] { "TenantId", "VentaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comprobantes_UserId",
                table: "comprobantes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_comprobantes_VentaId",
                table: "comprobantes",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_devolucion_items_DevolucionId",
                table: "devolucion_items",
                column: "DevolucionId");

            migrationBuilder.CreateIndex(
                name: "IX_devolucion_items_DevolucionId_ProductoId",
                table: "devolucion_items",
                columns: new[] { "DevolucionId", "ProductoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_devolucion_items_ProductoId",
                table: "devolucion_items",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_devolucion_items_TenantId",
                table: "devolucion_items",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_devoluciones_CreatedAt",
                table: "devoluciones",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_devoluciones_SucursalId",
                table: "devoluciones",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_devoluciones_TenantId",
                table: "devoluciones",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_devoluciones_UserId",
                table: "devoluciones",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_devoluciones_VentaId",
                table: "devoluciones",
                column: "VentaId");

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
                name: "IX_lista_precio_items_ListaPrecioId",
                table: "lista_precio_items",
                column: "ListaPrecioId");

            migrationBuilder.CreateIndex(
                name: "IX_lista_precio_items_ProductoId",
                table: "lista_precio_items",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_lista_precio_items_TenantId",
                table: "lista_precio_items",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_lista_precio_items_TenantId_ListaPrecioId_ProductoId",
                table: "lista_precio_items",
                columns: new[] { "TenantId", "ListaPrecioId", "ProductoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_listas_precio_TenantId",
                table: "listas_precio",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_listas_precio_TenantId_Nombre",
                table: "listas_precio",
                columns: new[] { "TenantId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_marcas_TenantId",
                table: "marcas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_nota_credito_interna_DevolucionId",
                table: "nota_credito_interna",
                column: "DevolucionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nota_credito_interna_SucursalId",
                table: "nota_credito_interna",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_nota_credito_interna_TenantId",
                table: "nota_credito_interna",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_orden_compra_items_OrdenCompraId",
                table: "orden_compra_items",
                column: "OrdenCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_orden_compra_items_OrdenCompraId_ProductoId",
                table: "orden_compra_items",
                columns: new[] { "OrdenCompraId", "ProductoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orden_compra_items_ProductoId",
                table: "orden_compra_items",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_orden_compra_items_TenantId",
                table: "orden_compra_items",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_compra_CreatedAt",
                table: "ordenes_compra",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_compra_Estado",
                table: "ordenes_compra",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_compra_ProveedorId",
                table: "ordenes_compra",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_compra_SucursalId",
                table: "ordenes_compra",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_compra_TenantId",
                table: "ordenes_compra",
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
                name: "IX_pre_recepcion_items_DocumentoCompraItemId",
                table: "pre_recepcion_items",
                column: "DocumentoCompraItemId");

            migrationBuilder.CreateIndex(
                name: "IX_pre_recepcion_items_PreRecepcionId",
                table: "pre_recepcion_items",
                column: "PreRecepcionId");

            migrationBuilder.CreateIndex(
                name: "IX_pre_recepcion_items_PreRecepcionId_DocumentoCompraItemId",
                table: "pre_recepcion_items",
                columns: new[] { "PreRecepcionId", "DocumentoCompraItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pre_recepcion_items_ProductoId",
                table: "pre_recepcion_items",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_pre_recepcion_items_TenantId",
                table: "pre_recepcion_items",
                column: "TenantId");

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

            migrationBuilder.CreateIndex(
                name: "IX_producto_codigos_ProductoId",
                table: "producto_codigos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_producto_codigos_TenantId",
                table: "producto_codigos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_producto_codigos_TenantId_Codigo",
                table: "producto_codigos",
                columns: new[] { "TenantId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_producto_proveedor_ProductoId",
                table: "producto_proveedor",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_producto_proveedor_ProveedorId",
                table: "producto_proveedor",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_producto_proveedor_TenantId",
                table: "producto_proveedor",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_producto_proveedor_TenantId_ProductoId",
                table: "producto_proveedor",
                columns: new[] { "TenantId", "ProductoId" },
                unique: true,
                filter: "\"EsPrincipal\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_producto_proveedor_TenantId_ProductoId_ProveedorId",
                table: "producto_proveedor",
                columns: new[] { "TenantId", "ProductoId", "ProveedorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_producto_stock_config_ProductoId",
                table: "producto_stock_config",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_producto_stock_config_SucursalId",
                table: "producto_stock_config",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_producto_stock_config_TenantId",
                table: "producto_stock_config",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_producto_stock_config_TenantId_ProductoId_SucursalId",
                table: "producto_stock_config",
                columns: new[] { "TenantId", "ProductoId", "SucursalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_productos_CategoriaId",
                table: "productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_productos_MarcaId",
                table: "productos",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_productos_ProveedorId",
                table: "productos",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_productos_TenantId",
                table: "productos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_productos_TenantId_Sku",
                table: "productos",
                columns: new[] { "TenantId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proveedores_TenantId",
                table: "proveedores",
                column: "TenantId");

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
                name: "IX_recepcion_items_RecepcionId_PreRecepcionItemId",
                table: "recepcion_items",
                columns: new[] { "RecepcionId", "PreRecepcionItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recepcion_items_TenantId",
                table: "recepcion_items",
                column: "TenantId");

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
                name: "IX_stock_movimiento_items_MovimientoId",
                table: "stock_movimiento_items",
                column: "MovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movimiento_items_ProductoId",
                table: "stock_movimiento_items",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movimiento_items_TenantId",
                table: "stock_movimiento_items",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movimientos_Fecha",
                table: "stock_movimientos",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movimientos_SucursalId",
                table: "stock_movimientos",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movimientos_TenantId",
                table: "stock_movimientos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_saldos_ProductoId",
                table: "stock_saldos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_saldos_SucursalId",
                table: "stock_saldos",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_saldos_TenantId",
                table: "stock_saldos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_saldos_TenantId_ProductoId_SucursalId",
                table: "stock_saldos",
                columns: new[] { "TenantId", "ProductoId", "SucursalId" },
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

            migrationBuilder.CreateIndex(
                name: "IX_venta_items_ProductoId",
                table: "venta_items",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_items_TenantId",
                table: "venta_items",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_items_VentaId",
                table: "venta_items",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_items_VentaId_ProductoId",
                table: "venta_items",
                columns: new[] { "VentaId", "ProductoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_venta_pagos_MedioPago",
                table: "venta_pagos",
                column: "MedioPago");

            migrationBuilder.CreateIndex(
                name: "IX_venta_pagos_TenantId",
                table: "venta_pagos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_venta_pagos_VentaId",
                table: "venta_pagos",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_CreatedAt",
                table: "ventas",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_Estado",
                table: "ventas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_SucursalId",
                table: "ventas",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_TenantId",
                table: "ventas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_UserId",
                table: "ventas",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "caja_movimientos");

            migrationBuilder.DropTable(
                name: "comprobantes");

            migrationBuilder.DropTable(
                name: "devolucion_items");

            migrationBuilder.DropTable(
                name: "lista_precio_items");

            migrationBuilder.DropTable(
                name: "nota_credito_interna");

            migrationBuilder.DropTable(
                name: "orden_compra_items");

            migrationBuilder.DropTable(
                name: "producto_codigos");

            migrationBuilder.DropTable(
                name: "producto_proveedor");

            migrationBuilder.DropTable(
                name: "producto_stock_config");

            migrationBuilder.DropTable(
                name: "recepcion_items");

            migrationBuilder.DropTable(
                name: "rol_permisos");

            migrationBuilder.DropTable(
                name: "stock_movimiento_items");

            migrationBuilder.DropTable(
                name: "stock_saldos");

            migrationBuilder.DropTable(
                name: "usuario_roles");

            migrationBuilder.DropTable(
                name: "venta_items");

            migrationBuilder.DropTable(
                name: "venta_pagos");

            migrationBuilder.DropTable(
                name: "caja_sesiones");

            migrationBuilder.DropTable(
                name: "listas_precio");

            migrationBuilder.DropTable(
                name: "devoluciones");

            migrationBuilder.DropTable(
                name: "ordenes_compra");

            migrationBuilder.DropTable(
                name: "pre_recepcion_items");

            migrationBuilder.DropTable(
                name: "recepciones");

            migrationBuilder.DropTable(
                name: "permisos");

            migrationBuilder.DropTable(
                name: "stock_movimientos");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "cajas");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "documento_compra_items");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "pre_recepciones");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "marcas");

            migrationBuilder.DropTable(
                name: "documentos_compra");

            migrationBuilder.DropTable(
                name: "proveedores");

            migrationBuilder.DropTable(
                name: "sucursales");

            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}
