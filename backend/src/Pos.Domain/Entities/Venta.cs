using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class Venta : EntityBase
{
    private Venta()
    {
    }

    public Venta(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid? userId,
        string listaPrecio,
        DateTimeOffset createdAtUtc,
        long numero = 0)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (string.IsNullOrWhiteSpace(listaPrecio)) throw new ArgumentException("ListaPrecio is required.", nameof(listaPrecio));

        SucursalId = sucursalId;
        UserId = userId;
        ListaPrecio = listaPrecio;
        Estado = VentaEstado.Borrador;
        Numero = numero;
    }

    public Guid SucursalId { get; private set; }
    public Guid? UserId { get; private set; }
    public long Numero { get; private set; }
    public string ListaPrecio { get; private set; } = string.Empty;
    public VentaEstado Estado { get; private set; }
    public decimal TotalNeto { get; private set; }
    public decimal TotalPagos { get; private set; }
    public bool Facturada { get; private set; }

    public void CambiarEstado(VentaEstado estado, DateTimeOffset updatedAtUtc)
    {
        Estado = estado;
        MarkUpdated(updatedAtUtc);
    }

    public void Confirmar(decimal totalNeto, decimal totalPagos, bool facturada, DateTimeOffset updatedAtUtc)
    {
        if (totalNeto < 0) throw new ArgumentException("TotalNeto must be >= 0.", nameof(totalNeto));
        if (totalPagos < 0) throw new ArgumentException("TotalPagos must be >= 0.", nameof(totalPagos));

        TotalNeto = totalNeto;
        TotalPagos = totalPagos;
        Facturada = facturada;
        Estado = VentaEstado.Confirmada;
        MarkUpdated(updatedAtUtc);
    }
}
