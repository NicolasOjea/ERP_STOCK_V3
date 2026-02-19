using Pos.Application.DTOs.Ventas;
using Pos.Application.DTOs.Pricing;

namespace Pos.Application.Abstractions;

public interface IVentaRepository
{
    Task<Guid> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid userId,
        string listaPrecio,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<VentaDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        CancellationToken cancellationToken = default);

    Task<VentaTicketDto?> GetTicketByNumeroAsync(
        Guid tenantId,
        Guid sucursalId,
        long numeroVenta,
        CancellationToken cancellationToken = default);

    Task<VentaItemChangeDto> AddItemByCodeAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        string code,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<VentaItemChangeDto> AddItemByProductAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        Guid productId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<VentaItemDto> RemoveItemAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        Guid itemId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<VentaItemChangeDto> UpdateItemCantidadAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        Guid itemId,
        decimal cantidad,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<VentaConfirmResultDto> ConfirmAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        VentaConfirmRequestDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<VentaAnularResultDto> AnularAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        VentaAnularRequestDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<VentaPricingSnapshotDto?> GetPricingSnapshotAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        CancellationToken cancellationToken = default);
}
