using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Recepcion : EntityBase
{
    private Recepcion()
    {
    }

    public Recepcion(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid preRecepcionId,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (preRecepcionId == Guid.Empty) throw new ArgumentException("PreRecepcionId is required.", nameof(preRecepcionId));

        SucursalId = sucursalId;
        PreRecepcionId = preRecepcionId;
    }

    public Guid SucursalId { get; private set; }
    public Guid PreRecepcionId { get; private set; }
}
