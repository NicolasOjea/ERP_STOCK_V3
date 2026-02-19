using Pos.Application.DTOs.Pricing;

namespace Pos.Application.UseCases.Pricing;

public sealed class PricingContext
{
    private readonly Dictionary<Guid, PricingItemState> _items;

    public PricingContext(IReadOnlyList<VentaPricingItemDto> items)
    {
        _items = items.ToDictionary(i => i.ItemId, i => new PricingItemState(i));
    }

    public IReadOnlyCollection<PricingItemState> Items => _items.Values;

    public PricingItemState GetItem(Guid itemId) => _items[itemId];
}

public sealed class PricingItemState
{
    public PricingItemState(VentaPricingItemDto item)
    {
        Item = item;
        DisponibleCantidad = item.Cantidad;
        Promos = new List<VentaRecalculoItemPromoDto>();
    }

    public VentaPricingItemDto Item { get; }
    public decimal DisponibleCantidad { get; private set; }
    public decimal DescuentoAcumulado { get; private set; }
    public List<VentaRecalculoItemPromoDto> Promos { get; }

    public decimal Bruto => PricingMath.Round(Item.Cantidad * Item.PrecioUnitario);

    public void Consume(decimal cantidad)
    {
        if (cantidad <= 0)
        {
            return;
        }

        if (DisponibleCantidad < cantidad)
        {
            cantidad = DisponibleCantidad;
        }

        DisponibleCantidad = PricingMath.Round(DisponibleCantidad - cantidad);
    }

    public void AddPromo(VentaRecalculoItemPromoDto promo)
    {
        if (promo.Descuento <= 0)
        {
            return;
        }

        Promos.Add(promo);
        DescuentoAcumulado = PricingMath.Round(DescuentoAcumulado + promo.Descuento);
    }
}

public static class PricingMath
{
    public static decimal Round(decimal value) =>
        Math.Round(value, 4, MidpointRounding.AwayFromZero);
}
