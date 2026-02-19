using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class PreRecepcion : EntityBase
{
    private PreRecepcion()
    {
    }

    public PreRecepcion(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid documentoCompraId,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (documentoCompraId == Guid.Empty) throw new ArgumentException("DocumentoCompraId is required.", nameof(documentoCompraId));

        SucursalId = sucursalId;
        DocumentoCompraId = documentoCompraId;
    }

    public Guid SucursalId { get; private set; }
    public Guid DocumentoCompraId { get; private set; }
}
