using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class NotaCreditoInterna : EntityBase
{
    private NotaCreditoInterna()
    {
    }

    public NotaCreditoInterna(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid devolucionId,
        decimal total,
        string motivo,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (devolucionId == Guid.Empty) throw new ArgumentException("DevolucionId is required.", nameof(devolucionId));
        if (total < 0) throw new ArgumentException("Total must be >= 0.", nameof(total));
        if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("Motivo is required.", nameof(motivo));

        SucursalId = sucursalId;
        DevolucionId = devolucionId;
        Total = total;
        Motivo = motivo;
    }

    public Guid SucursalId { get; private set; }
    public Guid DevolucionId { get; private set; }
    public decimal Total { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
}
