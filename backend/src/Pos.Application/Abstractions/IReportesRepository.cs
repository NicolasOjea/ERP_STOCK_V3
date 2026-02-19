using Pos.Application.DTOs.Reportes;

namespace Pos.Application.Abstractions;

public interface IReportesRepository
{
    Task<ReportResumenVentasDto> GetResumenVentasAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VentaPorDiaItemDto>> GetVentasPorDiaAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MedioPagoItemDto>> GetMediosPagoAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TopProductoItemDto>> GetTopProductosAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        int top,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RotacionStockItemDto>> GetRotacionStockAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockInmovilizadoItemDto>> GetStockInmovilizadoAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default);
}
