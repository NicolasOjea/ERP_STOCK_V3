using Pos.Application.DTOs.Compras;

namespace Pos.Application.Abstractions;

public interface IOrdenCompraRepository
{
    Task<Guid> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        OrdenCompraCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrdenCompraListItemDto>> GetListAsync(
        Guid tenantId,
        Guid sucursalId,
        CancellationToken cancellationToken = default);

    Task<OrdenCompraDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ordenCompraId,
        CancellationToken cancellationToken = default);

    Task<OrdenCompraDto> EnviarAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ordenCompraId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<OrdenCompraDto> CancelarAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ordenCompraId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
