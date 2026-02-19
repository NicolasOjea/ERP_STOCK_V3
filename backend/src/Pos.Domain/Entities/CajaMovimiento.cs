using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class CajaMovimiento : EntityBase
{
    private CajaMovimiento()
    {
    }

    public CajaMovimiento(
        Guid id,
        Guid tenantId,
        Guid cajaSesionId,
        CajaMovimientoTipo tipo,
        string medioPago,
        decimal monto,
        string motivo,
        DateTimeOffset fecha,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (cajaSesionId == Guid.Empty) throw new ArgumentException("CajaSesionId is required.", nameof(cajaSesionId));
        if (string.IsNullOrWhiteSpace(medioPago)) throw new ArgumentException("MedioPago is required.", nameof(medioPago));
        if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("Motivo is required.", nameof(motivo));

        CajaSesionId = cajaSesionId;
        Tipo = tipo;
        MedioPago = medioPago;
        Monto = monto;
        Motivo = motivo;
        Fecha = fecha;
    }

    public Guid CajaSesionId { get; private set; }
    public CajaMovimientoTipo Tipo { get; private set; }
    public string MedioPago { get; private set; } = string.Empty;
    public decimal Monto { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public DateTimeOffset Fecha { get; private set; }
}
