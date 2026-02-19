using Pos.Domain.Entities;
using Pos.Domain.Enums;

namespace Pos.Infrastructure.Persistence;

public static class SeedData
{
    public static readonly DateTimeOffset SeedTimestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static readonly Guid TenantId = Guid.Parse("1f4f9f1a-2b7f-4d2c-8cf6-56d5b92f1a01");
    public static readonly Guid SucursalId = Guid.Parse("3b58fbc1-33f3-4e7e-9a34-9a0a3cf3c7a1");
    public static readonly Guid AdminUserId = Guid.Parse("c5a5020f-47f8-4b57-8a2a-1f9a4c495e2b");

    public static readonly Guid RoleCajeroId = Guid.Parse("5e7f1a0b-8f29-4b6f-93a0-2f7a9f2c7b01");
    public static readonly Guid RoleEncargadoId = Guid.Parse("9b1b6f34-33d5-4a0c-9a11-0e2b9d5c1b02");
    public static readonly Guid RoleAdminId = Guid.Parse("e1c5d7b9-6a28-4c79-8d1f-7d1b0f2e3a03");

    public static Tenant Tenant => new Tenant(TenantId, "Demo", SeedTimestamp);

    public static Sucursal Sucursal => new Sucursal(
        SucursalId,
        TenantId,
        "Central",
        "CENTRAL",
        SeedTimestamp);

    public static User AdminUser => new User(
        AdminUserId,
        TenantId,
        "admin",
        "sha256:3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121",
        SeedTimestamp,
        true);

    public static Role[] Roles =>
    [
        new Role(RoleCajeroId, TenantId, "CAJERO", SeedTimestamp),
        new Role(RoleEncargadoId, TenantId, "ENCARGADO", SeedTimestamp),
        new Role(RoleAdminId, TenantId, "ADMIN", SeedTimestamp)
    ];

    public static Permission[] Permissions =>
    [
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a001"), TenantId, PermissionCodes.VentaCrear, "Crear venta", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a002"), TenantId, PermissionCodes.VentaConfirmar, "Confirmar venta", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a003"), TenantId, PermissionCodes.VentaAnular, "Anular venta", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a004"), TenantId, PermissionCodes.DevolucionRegistrar, "Registrar devolucion", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a005"), TenantId, PermissionCodes.CajaAbrir, "Abrir caja", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a006"), TenantId, PermissionCodes.CajaCerrar, "Cerrar caja", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a007"), TenantId, PermissionCodes.StockAjustar, "Ajustar stock", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a008"), TenantId, PermissionCodes.ProductoVer, "Ver productos", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a009"), TenantId, PermissionCodes.ProductoEditar, "Editar productos", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a010"), TenantId, PermissionCodes.UsuarioAdmin, "Administrar usuarios", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a011"), TenantId, PermissionCodes.ReportesVer, "Ver reportes", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a012"), TenantId, PermissionCodes.ClienteGestionar, "Gestionar clientes", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a013"), TenantId, PermissionCodes.ProveedorGestionar, "Gestionar proveedores", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a014"), TenantId, PermissionCodes.ComprasRegistrar, "Registrar compras", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a015"), TenantId, PermissionCodes.ConfiguracionVer, "Ver configuracion", SeedTimestamp),
        new Permission(Guid.Parse("d1d7a221-4b7b-4d6f-bb2f-0a01f0e1a016"), TenantId, PermissionCodes.CajaMovimiento, "Movimientos de caja", SeedTimestamp)
    ];

    public static UserRole[] UserRoles =>
    [
        new UserRole(Guid.Parse("f19b3d0a-92f1-4b7d-9d3f-05b0f8f1a101"), TenantId, AdminUserId, RoleAdminId, SeedTimestamp)
    ];

    public static RolePermission[] RolePermissions =>
    [
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c01"), TenantId, RoleCajeroId, Permissions[0].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c02"), TenantId, RoleCajeroId, Permissions[1].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c03"), TenantId, RoleCajeroId, Permissions[4].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c04"), TenantId, RoleCajeroId, Permissions[5].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c05"), TenantId, RoleEncargadoId, Permissions[0].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c06"), TenantId, RoleEncargadoId, Permissions[1].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c07"), TenantId, RoleEncargadoId, Permissions[2].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c08"), TenantId, RoleEncargadoId, Permissions[6].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c09"), TenantId, RoleEncargadoId, Permissions[7].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c10"), TenantId, RoleEncargadoId, Permissions[8].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c11"), TenantId, RoleAdminId, Permissions[9].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c12"), TenantId, RoleAdminId, Permissions[10].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c13"), TenantId, RoleAdminId, Permissions[11].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c14"), TenantId, RoleAdminId, Permissions[12].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c15"), TenantId, RoleAdminId, Permissions[13].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c16"), TenantId, RoleAdminId, Permissions[14].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c17"), TenantId, RoleEncargadoId, Permissions[15].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c18"), TenantId, RoleAdminId, Permissions[15].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c19"), TenantId, RoleAdminId, Permissions[0].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1a"), TenantId, RoleAdminId, Permissions[1].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1b"), TenantId, RoleAdminId, Permissions[2].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1c"), TenantId, RoleAdminId, Permissions[3].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1d"), TenantId, RoleAdminId, Permissions[4].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1e"), TenantId, RoleAdminId, Permissions[5].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c1f"), TenantId, RoleAdminId, Permissions[6].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c20"), TenantId, RoleAdminId, Permissions[7].Id, SeedTimestamp),
        new RolePermission(Guid.Parse("a01b2c3d-4e5f-6a7b-8c9d-0e1f2a3b4c21"), TenantId, RoleAdminId, Permissions[8].Id, SeedTimestamp)
    ];
}
