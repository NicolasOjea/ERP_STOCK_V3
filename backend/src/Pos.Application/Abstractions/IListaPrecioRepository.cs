using Pos.Application.DTOs.ListasPrecio;

namespace Pos.Application.Abstractions;

public interface IListaPrecioRepository
{
    Task<IReadOnlyList<ListaPrecioDto>> GetListAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<ListaPrecioDto?> GetByIdAsync(
        Guid tenantId,
        Guid listaPrecioId,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        Guid tenantId,
        ListaPrecioCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid tenantId,
        Guid listaPrecioId,
        ListaPrecioUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task UpsertItemsAsync(
        Guid tenantId,
        Guid listaPrecioId,
        IReadOnlyList<ListaPrecioItemUpsertDto> items,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
