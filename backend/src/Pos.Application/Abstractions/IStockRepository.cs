using Pos.Application.DTOs.Stock;

namespace Pos.Application.Abstractions;

public interface IStockRepository
{
    Task<bool> ProductExistsAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default);

    Task<StockConfigDto?> GetStockConfigAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<StockConfigDto> UpsertStockConfigAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid productId,
        decimal stockMinimo,
        decimal stockDeseado,
        decimal toleranciaPct,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockSaldoDto>> GetSaldosAsync(
        Guid tenantId,
        Guid sucursalId,
        string? search,
        Guid? proveedorId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockAlertaDto>> GetAlertasAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid? proveedorId,
        CancellationToken cancellationToken = default);

    Task<StockSugeridoCompraDto> GetSugeridoCompraAsync(
        Guid tenantId,
        Guid sucursalId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockRemitoProductoDto>> GetProductosRemitoAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> productoIds,
        Guid? proveedorId,
        CancellationToken cancellationToken = default);

    Task<StockRemitoHeaderDto> GetRemitoHeaderAsync(
        Guid tenantId,
        Guid sucursalId,
        CancellationToken cancellationToken = default);
}
