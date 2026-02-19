using Pos.Application.DTOs.Etiquetas;
using Pos.Application.DTOs.Products;

namespace Pos.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductListItemDto>> SearchAsync(
        Guid tenantId,
        string? search,
        Guid? categoriaId,
        bool? activo,
        CancellationToken cancellationToken = default);

    Task<ProductDetailDto?> GetByIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        Guid tenantId,
        ProductCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Guid tenantId,
        Guid productId,
        ProductUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<ProductCodeDto?> AddCodeAsync(
        Guid tenantId,
        Guid productId,
        string code,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<ProductCodeDto?> RemoveCodeAsync(
        Guid tenantId,
        Guid productId,
        Guid codeId,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid tenantId,
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<ProductProveedorDto?> AddProveedorAsync(
        Guid tenantId,
        Guid productId,
        Guid proveedorId,
        bool esPrincipal,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<ProductProveedorDto?> SetProveedorPrincipalAsync(
        Guid tenantId,
        Guid productId,
        Guid relationId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetIdBySkuAsync(
        Guid tenantId,
        string sku,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetIdByCodeAsync(
        Guid tenantId,
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DTOs.Etiquetas.EtiquetaItemDto>> GetLabelDataAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> productIds,
        string listaPrecio,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CodigoBarraProductoDto>> GetBarcodeDataAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken = default);
}
