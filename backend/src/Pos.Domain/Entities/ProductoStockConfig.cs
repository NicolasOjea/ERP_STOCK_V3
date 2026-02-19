using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class ProductoStockConfig : EntityBase
{
    private ProductoStockConfig()
    {
    }

    public ProductoStockConfig(
        Guid id,
        Guid tenantId,
        Guid productoId,
        Guid sucursalId,
        decimal stockMinimo,
        decimal stockDeseado,
        decimal toleranciaPct,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));

        ProductoId = productoId;
        SucursalId = sucursalId;
        StockMinimo = stockMinimo;
        StockDeseado = stockDeseado;
        ToleranciaPct = toleranciaPct;
    }

    public Guid ProductoId { get; private set; }
    public Guid SucursalId { get; private set; }
    public decimal StockMinimo { get; private set; }
    public decimal StockDeseado { get; private set; }
    public decimal ToleranciaPct { get; private set; }

    public void Update(decimal stockMinimo, decimal stockDeseado, decimal toleranciaPct, DateTimeOffset updatedAtUtc)
    {
        StockMinimo = stockMinimo;
        StockDeseado = stockDeseado;
        ToleranciaPct = toleranciaPct;
        MarkUpdated(updatedAtUtc);
    }
}
