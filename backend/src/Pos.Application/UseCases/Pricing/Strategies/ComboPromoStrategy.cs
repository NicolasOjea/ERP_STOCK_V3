using Pos.Application.DTOs.Pricing;
using Pos.Domain.Enums;

namespace Pos.Application.UseCases.Pricing.Strategies;

public sealed class ComboPromoStrategy : IPromoStrategy
{
    public PromocionTipo Tipo => PromocionTipo.Combo;

    public PromoApplyResult Apply(PromocionAplicableDto promo, PricingContext context)
    {
        var comboItems = promo.ComboItems ?? Array.Empty<PromoComboItemDto>();
        if (comboItems.Count == 0)
        {
            return PromoApplyResult.Empty;
        }

        if (comboItems.Any(i => i.Cantidad <= 0))
        {
            return PromoApplyResult.Empty;
        }

        var itemsByProduct = context.Items
            .Where(i => i.DisponibleCantidad > 0)
            .GroupBy(i => i.Item.ProductoId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var comboItem in comboItems)
        {
            if (!itemsByProduct.ContainsKey(comboItem.ProductoId))
            {
                return PromoApplyResult.Empty;
            }
        }

        var comboCount = int.MaxValue;
        foreach (var comboItem in comboItems)
        {
            var disponibles = itemsByProduct[comboItem.ProductoId].Sum(i => i.DisponibleCantidad);
            var maxForItem = (int)Math.Floor(disponibles / comboItem.Cantidad);
            comboCount = Math.Min(comboCount, maxForItem);
        }

        if (comboCount <= 0 || comboCount == int.MaxValue)
        {
            return PromoApplyResult.Empty;
        }

        var allocations = new List<ComboAllocation>();
        foreach (var comboItem in comboItems)
        {
            var requiredTotal = comboItem.Cantidad * comboCount;
            var remaining = requiredTotal;
            foreach (var state in itemsByProduct[comboItem.ProductoId])
            {
                if (remaining <= 0)
                {
                    break;
                }

                var take = Math.Min(state.DisponibleCantidad, remaining);
                if (take <= 0)
                {
                    continue;
                }

                state.Consume(take);
                remaining -= take;

                allocations.Add(new ComboAllocation(state.Item.ItemId, take, state.Item.PrecioUnitario));
            }
        }

        var totalBruto = PricingMath.Round(allocations.Sum(a => a.Cantidad * a.PrecioUnitario));
        if (totalBruto <= 0)
        {
            return PromoApplyResult.Empty;
        }

        var totalDescuento = PricingMath.Round(totalBruto * promo.Porcentaje / 100m);
        if (totalDescuento <= 0)
        {
            return PromoApplyResult.Empty;
        }

        var discounts = new List<PromoItemDiscount>();
        var remainingDescuento = totalDescuento;
        for (var i = 0; i < allocations.Count; i++)
        {
            var allocation = allocations[i];
            var bruto = PricingMath.Round(allocation.Cantidad * allocation.PrecioUnitario);
            if (bruto <= 0)
            {
                continue;
            }

            decimal descuento;
            if (i == allocations.Count - 1)
            {
                descuento = remainingDescuento;
            }
            else
            {
                descuento = PricingMath.Round(totalDescuento * (bruto / totalBruto));
                remainingDescuento = PricingMath.Round(remainingDescuento - descuento);
            }

            if (descuento <= 0)
            {
                continue;
            }

            discounts.Add(new PromoItemDiscount(allocation.ItemId, descuento));
        }

        return discounts.Count == 0
            ? PromoApplyResult.Empty
            : new PromoApplyResult(discounts);
    }

    private sealed record ComboAllocation(Guid ItemId, decimal Cantidad, decimal PrecioUnitario);
}
