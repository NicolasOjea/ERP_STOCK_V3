using Pos.Application.DTOs.Stock;
using Pos.Domain.Enums;

namespace Pos.Application.Abstractions;

public interface IStockMovementRepository
{
    Task<StockMovimientoRegisterResult> RegisterAsync(
        Guid tenantId,
        Guid sucursalId,
        StockMovimientoTipo tipo,
        string motivo,
        IReadOnlyCollection<StockMovimientoItemCreateDto> items,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockMovimientoDto>> SearchAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid? productoId,
        long? ventaNumero,
        bool? facturada,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default);
}
