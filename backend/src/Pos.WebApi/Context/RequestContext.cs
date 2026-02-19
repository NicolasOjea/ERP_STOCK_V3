using Pos.Application.Abstractions;

namespace Pos.WebApi.Context;

public sealed class RequestContext : IRequestContext
{
    public Guid TenantId { get; private set; } = Guid.Empty;
    public Guid SucursalId { get; private set; } = Guid.Empty;
    public Guid UserId { get; private set; } = Guid.Empty;

    public void Set(Guid tenantId, Guid sucursalId, Guid userId)
    {
        TenantId = tenantId;
        SucursalId = sucursalId;
        UserId = userId;
    }
}
