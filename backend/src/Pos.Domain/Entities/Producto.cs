using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class Producto : EntityBase
{
    private Producto()
    {
    }

    public Producto(
        Guid id,
        Guid tenantId,
        string name,
        string sku,
        Guid? categoriaId,
        Guid? marcaId,
        Guid? proveedorId,
        DateTimeOffset createdAtUtc,
        decimal precioBase = 1m,
        decimal precioVenta = 1m,
        ProductPricingMode pricingMode = ProductPricingMode.FijoPct,
        decimal? margenGananciaPct = null,
        bool isActive = true)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("Sku is required.", nameof(sku));
        if (precioBase < 0) throw new ArgumentException("PrecioBase must be >= 0.", nameof(precioBase));
        if (precioVenta < 0) throw new ArgumentException("PrecioVenta must be >= 0.", nameof(precioVenta));

        Name = name;
        Sku = sku;
        CategoriaId = categoriaId;
        MarcaId = marcaId;
        ProveedorId = proveedorId;
        PrecioBase = precioBase;
        PrecioVenta = precioVenta;
        PricingMode = pricingMode;
        MargenGananciaPct = margenGananciaPct;
        IsActive = isActive;
    }

    public Producto(
        Guid id,
        Guid tenantId,
        string name,
        string sku,
        Guid? categoriaId,
        Guid? marcaId,
        Guid? proveedorId,
        DateTimeOffset createdAtUtc,
        decimal precioBase,
        decimal precioVenta,
        bool isActive)
        : this(
            id,
            tenantId,
            name,
            sku,
            categoriaId,
            marcaId,
            proveedorId,
            createdAtUtc,
            precioBase,
            precioVenta,
            ProductPricingMode.FijoPct,
            null,
            isActive)
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public Guid? CategoriaId { get; private set; }
    public Guid? MarcaId { get; private set; }
    public Guid? ProveedorId { get; private set; }
    public decimal PrecioBase { get; private set; }
    public decimal PrecioVenta { get; private set; }
    public ProductPricingMode PricingMode { get; private set; }
    public decimal? MargenGananciaPct { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(
        string name,
        string sku,
        Guid? categoriaId,
        Guid? marcaId,
        Guid? proveedorId,
        decimal precioBase,
        decimal precioVenta,
        ProductPricingMode pricingMode,
        decimal? margenGananciaPct,
        bool isActive,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("Sku is required.", nameof(sku));
        if (precioBase < 0) throw new ArgumentException("PrecioBase must be >= 0.", nameof(precioBase));
        if (precioVenta < 0) throw new ArgumentException("PrecioVenta must be >= 0.", nameof(precioVenta));

        Name = name;
        Sku = sku;
        CategoriaId = categoriaId;
        MarcaId = marcaId;
        ProveedorId = proveedorId;
        PrecioBase = precioBase;
        PrecioVenta = precioVenta;
        PricingMode = pricingMode;
        MargenGananciaPct = margenGananciaPct;
        IsActive = isActive;
        MarkUpdated(updatedAtUtc);
    }

    public void SetProveedorPrincipal(Guid? proveedorId, DateTimeOffset updatedAtUtc)
    {
        ProveedorId = proveedorId;
        MarkUpdated(updatedAtUtc);
    }
}
