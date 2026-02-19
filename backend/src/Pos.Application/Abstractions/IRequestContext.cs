namespace Pos.Application.Abstractions;

public interface IRequestContext
{
    Guid TenantId { get; }
    Guid SucursalId { get; }
    Guid UserId { get; }

    void Set(Guid tenantId, Guid sucursalId, Guid userId);
}
