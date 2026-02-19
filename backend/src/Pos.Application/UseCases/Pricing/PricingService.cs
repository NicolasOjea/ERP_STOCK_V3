using Pos.Application.DTOs.Pricing;
using Pos.Domain.Enums;

namespace Pos.Application.UseCases.Pricing;

public sealed class PricingService
{
    private readonly IReadOnlyDictionary<PromocionTipo, IPromoStrategy> _strategies;

    public PricingService(IEnumerable<IPromoStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Tipo, s => s);
    }

    public VentaRecalculoDto Recalcular(
        VentaPricingSnapshotDto venta,
        IReadOnlyList<PromocionAplicableDto> promociones)
    {
        var context = new PricingContext(venta.Items);
        var promoResumen = new Dictionary<Guid, VentaRecalculoPromoDto>();

        var promosOrdenadas = promociones
            .OrderByDescending(p => p.Prioridad)
            .ThenByDescending(p => p.Porcentaje)
            .ToList();

        foreach (var promo in promosOrdenadas)
        {
            if (!_strategies.TryGetValue(promo.Tipo, out var strategy))
            {
                continue;
            }

            var applyResult = strategy.Apply(promo, context);
            if (applyResult.ItemDiscounts.Count == 0)
            {
                continue;
            }

            var tipo = ToTipoString(promo.Tipo);
            var descuentoPorItem = applyResult.ItemDiscounts
                .GroupBy(d => d.ItemId)
                .Select(g => new { ItemId = g.Key, Descuento = PricingMath.Round(g.Sum(x => x.Descuento)) })
                .Where(d => d.Descuento > 0)
                .ToList();

            decimal totalDescuentoPromo = 0m;

            foreach (var item in descuentoPorItem)
            {
                if (!TryAddPromo(context, item.ItemId, promo, tipo, item.Descuento))
                {
                    continue;
                }

                totalDescuentoPromo = PricingMath.Round(totalDescuentoPromo + item.Descuento);
            }

            if (totalDescuentoPromo <= 0)
            {
                continue;
            }

            if (promoResumen.TryGetValue(promo.Id, out var existing))
            {
                promoResumen[promo.Id] = existing with { TotalDescuento = PricingMath.Round(existing.TotalDescuento + totalDescuentoPromo) };
            }
            else
            {
                promoResumen[promo.Id] = new VentaRecalculoPromoDto(
                    promo.Id,
                    promo.Nombre,
                    tipo,
                    promo.Porcentaje,
                    totalDescuentoPromo);
            }
        }

        var items = context.Items
            .Select(state =>
            {
                var bruto = state.Bruto;
                var descuento = state.DescuentoAcumulado;
                var neto = PricingMath.Round(bruto - descuento);

                return new VentaRecalculoItemDto(
                    state.Item.ItemId,
                    state.Item.ProductoId,
                    state.Item.Nombre,
                    state.Item.Sku,
                    state.Item.CategoriaId,
                    state.Item.Cantidad,
                    state.Item.PrecioUnitario,
                    bruto,
                    descuento,
                    neto,
                    state.Promos);
            })
            .ToList();

        var totalBruto = PricingMath.Round(items.Sum(i => i.Bruto));
        var totalDescuento = PricingMath.Round(items.Sum(i => i.Descuento));
        var totalNeto = PricingMath.Round(totalBruto - totalDescuento);

        return new VentaRecalculoDto(
            venta.VentaId,
            totalBruto,
            totalDescuento,
            totalNeto,
            items,
            promoResumen.Values.OrderByDescending(p => p.TotalDescuento).ToList());
    }

    private static bool TryAddPromo(
        PricingContext context,
        Guid itemId,
        PromocionAplicableDto promo,
        string tipo,
        decimal descuento)
    {
        if (descuento <= 0)
        {
            return false;
        }

        var state = context.GetItem(itemId);
        state.AddPromo(new VentaRecalculoItemPromoDto(
            promo.Id,
            promo.Nombre,
            tipo,
            promo.Porcentaje,
            descuento));

        return true;
    }

    private static string ToTipoString(PromocionTipo tipo)
    {
        var name = tipo.ToString();
        var chars = new List<char>(name.Length * 2);
        for (var i = 0; i < name.Length; i++)
        {
            var ch = name[i];
            if (i > 0 && char.IsUpper(ch))
            {
                chars.Add('_');
            }

            chars.Add(char.ToUpperInvariant(ch));
        }

        return new string(chars.ToArray());
    }
}
