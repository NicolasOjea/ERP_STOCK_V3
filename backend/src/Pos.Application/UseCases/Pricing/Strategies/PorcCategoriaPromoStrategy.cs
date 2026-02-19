using Pos.Application.DTOs.Pricing;
using Pos.Domain.Enums;

namespace Pos.Application.UseCases.Pricing.Strategies;

public sealed class PorcCategoriaPromoStrategy : IPromoStrategy
{
    public PromocionTipo Tipo => PromocionTipo.PorcCategoria;

    public PromoApplyResult Apply(PromocionAplicableDto promo, PricingContext context)
    {
        if (promo.CategoriaIds.Count == 0)
        {
            return PromoApplyResult.Empty;
        }

        var discounts = new List<PromoItemDiscount>();

        foreach (var state in context.Items)
        {
            if (!state.Item.CategoriaId.HasValue)
            {
                continue;
            }

            if (!promo.CategoriaIds.Contains(state.Item.CategoriaId.Value))
            {
                continue;
            }

            if (state.DisponibleCantidad <= 0)
            {
                continue;
            }

            var bruto = PricingMath.Round(state.DisponibleCantidad * state.Item.PrecioUnitario);
            if (bruto <= 0)
            {
                continue;
            }

            var descuento = PricingMath.Round(bruto * promo.Porcentaje / 100m);
            if (descuento <= 0)
            {
                continue;
            }

            discounts.Add(new PromoItemDiscount(state.Item.ItemId, descuento));
            state.Consume(state.DisponibleCantidad);
        }

        return discounts.Count == 0
            ? PromoApplyResult.Empty
            : new PromoApplyResult(discounts);
    }
}
