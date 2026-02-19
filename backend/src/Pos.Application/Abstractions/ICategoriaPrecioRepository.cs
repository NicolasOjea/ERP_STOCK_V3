using Pos.Application.DTOs.Categorias;

namespace Pos.Application.Abstractions;

public interface ICategoriaPrecioRepository
{
    Task<IReadOnlyList<CategoriaPrecioDto>> SearchAsync(
        Guid tenantId,
        string? search,
        bool? activo,
        CancellationToken cancellationToken = default);

    Task<CategoriaPrecioDto?> GetByIdAsync(
        Guid tenantId,
        Guid categoriaId,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        Guid tenantId,
        CategoriaPrecioCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid tenantId,
        Guid categoriaId,
        CategoriaPrecioUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
