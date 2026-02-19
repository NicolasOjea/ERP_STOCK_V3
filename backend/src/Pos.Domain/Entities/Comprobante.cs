using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class Comprobante : EntityBase
{
    private Comprobante()
    {
    }

    public Comprobante(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        Guid? userId,
        decimal total,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (ventaId == Guid.Empty) throw new ArgumentException("VentaId is required.", nameof(ventaId));
        if (total < 0) throw new ArgumentException("Total must be >= 0.", nameof(total));

        SucursalId = sucursalId;
        VentaId = ventaId;
        UserId = userId;
        Total = total;
        Estado = ComprobanteEstado.Borrador;
    }

    public Guid SucursalId { get; private set; }
    public Guid VentaId { get; private set; }
    public Guid? UserId { get; private set; }
    public ComprobanteEstado Estado { get; private set; }
    public decimal Total { get; private set; }
    public string? Numero { get; private set; }
    public string? FiscalProvider { get; private set; }
    public string? FiscalPayload { get; private set; }
    public DateTimeOffset? EmitidoAt { get; private set; }

    public void Emitir(
        string provider,
        string numero,
        string? payload,
        DateTimeOffset emitidoAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(provider)) throw new ArgumentException("Provider is required.", nameof(provider));
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Numero is required.", nameof(numero));

        Estado = ComprobanteEstado.Emitido;
        FiscalProvider = provider;
        Numero = numero;
        FiscalPayload = payload;
        EmitidoAt = emitidoAtUtc;
        MarkUpdated(updatedAtUtc);
    }
}
