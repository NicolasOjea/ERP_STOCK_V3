using Pos.Application.DTOs.Pricing;
using Pos.Domain.Enums;

namespace Pos.Application.UseCases.Pricing.Strategies;

public sealed class DosPorUnoPromoStrategy : IPromoStrategy
{
    public PromocionTipo Tipo => PromocionTipo.DosPorUno;

    public PromoApplyResult Apply(PromocionAplicableDto promo, PricingContext context)
    {
        // Regla: 2x1 = se bonifica la unidad mas barata de cada par.
        // Se consumen ambas unidades del par para evitar stacking con otras promos.
        var productoIds = promo.ProductoIds ?? Array.Empty<Guid>();
        var categoriaIds = promo.CategoriaIds;

        if (productoIds.Count == 0 && categoriaIds.Count == 0)
        {
            return PromoApplyResult.Empty;
        }

        var eligible = context.Items
            .Where(i => i.DisponibleCantidad >= 1m)
            .Where(i =>
                (productoIds.Count > 0 && productoIds.Contains(i.Item.ProductoId)) ||
                (productoIds.Count == 0 && i.Item.CategoriaId.HasValue && categoriaIds.Contains(i.Item.CategoriaId.Value)))
            .ToList();

        if (eligible.Count == 0)
        {
            return PromoApplyResult.Empty;
        }

        var units = new List<UnitRef>();
        var index = 0;
        foreach (var state in eligible)
        {
            var unitCount = (int)Math.Floor(state.DisponibleCantidad);
            if (unitCount <= 0)
            {
                continue;
            }

            for (var i = 0; i < unitCount; i++)
            {
                units.Add(new UnitRef(index++, state.Item.ItemId, state.Item.PrecioUnitario));
            }
        }

        if (units.Count < 2)
        {
            return PromoApplyResult.Empty;
        }

        units.Sort(static (a, b) => a.Precio.CompareTo(b.Precio));

        var freeCount = units.Count / 2;
        if (freeCount <= 0)
        {
            return PromoApplyResult.Empty;
        }

        var freeIndexes = new HashSet<int>();
        for (var i = 0; i < freeCount; i++)
        {
            freeIndexes.Add(units[i].Index);
        }

        var paidIndexes = new HashSet<int>();
        for (var i = units.Count - 1; i >= 0 && paidIndexes.Count < freeCount; i--)
        {
            if (freeIndexes.Contains(units[i].Index))
            {
                continue;
            }

            paidIndexes.Add(units[i].Index);
        }

        var discountByItem = new Dictionary<Guid, decimal>();
        var consumeByItem = new Dictionary<Guid, int>();

        foreach (var unit in units.Where(u => freeIndexes.Contains(u.Index)))
        {
            discountByItem[unit.ItemId] = discountByItem.TryGetValue(unit.ItemId, out var acc)
                ? acc + unit.Precio
                : unit.Precio;

            consumeByItem[unit.ItemId] = consumeByItem.TryGetValue(unit.ItemId, out var count)
                ? count + 1
                : 1;
        }

        foreach (var unit in units.Where(u => paidIndexes.Contains(u.Index)))
        {
            consumeByItem[unit.ItemId] = consumeByItem.TryGetValue(unit.ItemId, out var count)
                ? count + 1
                : 1;
        }

        foreach (var kvp in consumeByItem)
        {
            context.GetItem(kvp.Key).Consume(kvp.Value);
        }

        var discounts = discountByItem
            .Select(kvp => new PromoItemDiscount(kvp.Key, PricingMath.Round(kvp.Value)))
            .Where(d => d.Descuento > 0)
            .ToList();

        return discounts.Count == 0
            ? PromoApplyResult.Empty
            : new PromoApplyResult(discounts);
    }

    private sealed record UnitRef(int Index, Guid ItemId, decimal Precio);
}
