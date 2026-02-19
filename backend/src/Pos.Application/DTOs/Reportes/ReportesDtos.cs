namespace Pos.Application.DTOs.Reportes;

public sealed record ReportSerieDto(string Name, IReadOnlyList<decimal> Data);

public sealed record ReportChartDto(
    IReadOnlyList<string> Labels,
    IReadOnlyList<ReportSerieDto> Series);

public sealed record ReportTableDto<T>(IReadOnlyList<T> Rows);

public sealed record ReportResumenVentasDto(
    decimal TotalIngresos,
    decimal TotalEgresos,
    decimal TotalFacturado,
    decimal TotalNoFacturado);

public sealed record VentaPorDiaItemDto(
    DateTime Fecha,
    decimal Total);

public sealed record MedioPagoItemDto(
    string MedioPago,
    decimal Total);

public sealed record TopProductoItemDto(
    Guid ProductoId,
    string Nombre,
    string Sku,
    decimal Cantidad,
    decimal Total);

public sealed record RotacionStockItemDto(
    Guid ProductoId,
    string Nombre,
    string Sku,
    decimal Entradas,
    decimal Salidas,
    decimal Neto);

public sealed record StockInmovilizadoItemDto(
    Guid ProductoId,
    string Nombre,
    string Sku,
    decimal StockActual,
    DateTimeOffset? UltimoMovimiento,
    int DiasSinMovimiento);
