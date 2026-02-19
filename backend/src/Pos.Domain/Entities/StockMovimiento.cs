using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class StockMovimiento : EntityBase
{
    private StockMovimiento()
    {
    }

    public StockMovimiento(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        StockMovimientoTipo tipo,
        string motivo,
        DateTimeOffset fecha,
        DateTimeOffset createdAtUtc,
        long? ventaNumero = null)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("Motivo is required.", nameof(motivo));

        SucursalId = sucursalId;
        Tipo = tipo;
        Motivo = motivo;
        Fecha = fecha;
        VentaNumero = ventaNumero;
    }

    public Guid SucursalId { get; private set; }
    public StockMovimientoTipo Tipo { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public DateTimeOffset Fecha { get; private set; }
    public long? VentaNumero { get; private set; }
}
