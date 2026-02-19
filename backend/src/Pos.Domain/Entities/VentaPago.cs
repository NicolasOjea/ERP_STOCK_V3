using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class VentaPago : EntityBase
{
    private VentaPago()
    {
    }

    public VentaPago(
        Guid id,
        Guid tenantId,
        Guid ventaId,
        string medioPago,
        decimal monto,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (ventaId == Guid.Empty) throw new ArgumentException("VentaId is required.", nameof(ventaId));
        if (string.IsNullOrWhiteSpace(medioPago)) throw new ArgumentException("MedioPago is required.", nameof(medioPago));
        if (monto <= 0) throw new ArgumentException("Monto must be greater than 0.", nameof(monto));

        VentaId = ventaId;
        MedioPago = medioPago;
        Monto = monto;
    }

    public Guid VentaId { get; private set; }
    public string MedioPago { get; private set; } = string.Empty;
    public decimal Monto { get; private set; }
}
