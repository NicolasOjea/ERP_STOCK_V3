using Pos.Domain.Enums;

namespace Pos.Application.DTOs.Pricing;

public sealed record PromocionAplicableDto(
    Guid Id,
    string Nombre,
    PromocionTipo Tipo,
    decimal Porcentaje,
    int Prioridad,
    IReadOnlyCollection<Guid> CategoriaIds,
    IReadOnlyCollection<Guid>? ProductoIds = null,
    IReadOnlyList<PromoComboItemDto>? ComboItems = null);

public sealed record PromoComboItemDto(
    Guid ProductoId,
    decimal Cantidad);

public sealed record VentaPricingItemDto(
    Guid ItemId,
    Guid ProductoId,
    Guid? CategoriaId,
    string Nombre,
    string Sku,
    decimal Cantidad,
    decimal PrecioUnitario);

public sealed record VentaPricingSnapshotDto(
    Guid VentaId,
    string ListaPrecio,
    IReadOnlyList<VentaPricingItemDto> Items);

public sealed record VentaRecalculoItemPromoDto(
    Guid PromocionId,
    string Nombre,
    string Tipo,
    decimal Porcentaje,
    decimal Descuento);

public sealed record VentaRecalculoItemDto(
    Guid ItemId,
    Guid ProductoId,
    string Nombre,
    string Sku,
    Guid? CategoriaId,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal Bruto,
    decimal Descuento,
    decimal Neto,
    IReadOnlyList<VentaRecalculoItemPromoDto> Promos);

public sealed record VentaRecalculoPromoDto(
    Guid PromocionId,
    string Nombre,
    string Tipo,
    decimal Porcentaje,
    decimal TotalDescuento);

public sealed record VentaRecalculoDto(
    Guid VentaId,
    decimal TotalBruto,
    decimal TotalDescuento,
    decimal TotalNeto,
    IReadOnlyList<VentaRecalculoItemDto> Items,
    IReadOnlyList<VentaRecalculoPromoDto> PromosAplicadas);
