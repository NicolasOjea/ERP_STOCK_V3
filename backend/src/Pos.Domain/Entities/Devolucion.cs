using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Devolucion : EntityBase
{
    private Devolucion()
    {
    }

    public Devolucion(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        Guid? userId,
        string motivo,
        decimal total,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (ventaId == Guid.Empty) throw new ArgumentException("VentaId is required.", nameof(ventaId));
        if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("Motivo is required.", nameof(motivo));
        if (total < 0) throw new ArgumentException("Total must be >= 0.", nameof(total));

        SucursalId = sucursalId;
        VentaId = ventaId;
        UserId = userId;
        Motivo = motivo;
        Total = total;
    }

    public Guid SucursalId { get; private set; }
    public Guid VentaId { get; private set; }
    public Guid? UserId { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
}
