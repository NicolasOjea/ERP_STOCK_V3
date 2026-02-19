using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Products;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Products;

public sealed class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public ProductService(
        IProductRepository productRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _productRepository = productRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<IReadOnlyList<ProductListItemDto>> SearchAsync(
        string? search,
        Guid? categoriaId,
        bool? activo,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        return await _productRepository.SearchAsync(tenantId, search, categoriaId, activo, cancellationToken);
    }

    public async Task<ProductDetailDto> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productId"] = new[] { "El producto es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var product = await _productRepository.GetByIdAsync(tenantId, productId, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        return product;
    }

    public async Task<ProductDetailDto> CreateAsync(ProductCreateDto request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["request"] = new[] { "El request es obligatorio." }
                });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["name"] = new[] { "El nombre es obligatorio." }
                });
        }

        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["sku"] = new[] { "El SKU es obligatorio." }
                });
        }

        if (request.CategoriaId.HasValue && request.CategoriaId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["categoriaId"] = new[] { "La categoria es invalida." }
                });
        }

        if (request.MarcaId.HasValue && request.MarcaId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["marcaId"] = new[] { "La marca es invalida." }
                });
        }

        if (!request.ProveedorId.HasValue || request.ProveedorId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor es obligatorio." }
                });
        }

        if (request.PrecioBase.HasValue && request.PrecioBase.Value < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["precioBase"] = new[] { "El precio base no puede ser negativo." }
                });
        }

        if (request.PrecioVenta.HasValue && request.PrecioVenta.Value < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["precioVenta"] = new[] { "El precio de venta no puede ser negativo." }
                });
        }

        if (request.MargenGananciaPct.HasValue && request.MargenGananciaPct.Value < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["margenGananciaPct"] = new[] { "El margen no puede ser negativo." }
                });
        }

        var tenantId = EnsureTenant();
        var now = DateTimeOffset.UtcNow;

        var normalizedRequest = request with
        {
            Name = request.Name.Trim(),
            Sku = request.Sku.Trim()
        };

        var newId = await _productRepository.CreateAsync(tenantId, normalizedRequest, now, cancellationToken);
        var created = await _productRepository.GetByIdAsync(tenantId, newId, cancellationToken);
        if (created is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        await _auditLogService.LogAsync(
            "Producto",
            created.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(created),
            null,
            cancellationToken);

        return created;
    }

    public async Task<ProductDetailDto> UpdateAsync(Guid productId, ProductUpdateDto request, CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productId"] = new[] { "El producto es obligatorio." }
                });
        }

        if (request is null)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["request"] = new[] { "El request es obligatorio." }
                });
        }

        var hasAnyChange = request.Name is not null
            || request.Sku is not null
            || request.CategoriaId is not null
            || request.MarcaId is not null
            || request.ProveedorId is not null
            || request.IsActive is not null
            || request.PrecioBase is not null
            || request.PrecioVenta is not null
            || request.PricingMode is not null
            || request.MargenGananciaPct is not null;

        if (!hasAnyChange)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["request"] = new[] { "Debe enviar al menos un cambio." }
                });
        }

        if (request.Name is not null && string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["name"] = new[] { "El nombre no puede estar vacio." }
                });
        }

        if (request.Sku is not null && string.IsNullOrWhiteSpace(request.Sku))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["sku"] = new[] { "El SKU no puede estar vacio." }
                });
        }

        if (request.CategoriaId.HasValue && request.CategoriaId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["categoriaId"] = new[] { "La categoria es invalida." }
                });
        }

        if (request.MarcaId.HasValue && request.MarcaId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["marcaId"] = new[] { "La marca es invalida." }
                });
        }

        if (request.ProveedorId.HasValue && request.ProveedorId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor es invalido." }
                });
        }

        if (request.PrecioBase.HasValue && request.PrecioBase.Value < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["precioBase"] = new[] { "El precio base no puede ser negativo." }
                });
        }

        if (request.PrecioVenta.HasValue && request.PrecioVenta.Value < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["precioVenta"] = new[] { "El precio de venta no puede ser negativo." }
                });
        }

        if (request.MargenGananciaPct.HasValue && request.MargenGananciaPct.Value < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["margenGananciaPct"] = new[] { "El margen no puede ser negativo." }
                });
        }

        var tenantId = EnsureTenant();
        var before = await _productRepository.GetByIdAsync(tenantId, productId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        var normalizedRequest = request with
        {
            Name = request.Name?.Trim(),
            Sku = request.Sku?.Trim()
        };

        var updated = await _productRepository.UpdateAsync(tenantId, productId, normalizedRequest, DateTimeOffset.UtcNow, cancellationToken);
        if (!updated)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        var after = await _productRepository.GetByIdAsync(tenantId, productId, cancellationToken);
        if (after is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        await _auditLogService.LogAsync(
            "Producto",
            after.Id.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(after),
            null,
            cancellationToken);

        return after;
    }

    public async Task<ProductCodeDto> AddCodeAsync(Guid productId, ProductCodeCreateDto request, CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productId"] = new[] { "El producto es obligatorio." }
                });
        }

        if (request is null || string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["code"] = new[] { "El codigo es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var code = request.Code.Trim();
        var result = await _productRepository.AddCodeAsync(tenantId, productId, code, DateTimeOffset.UtcNow, cancellationToken);
        if (result is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        await _auditLogService.LogAsync(
            "ProductoCodigo",
            result.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(result),
            JsonSerializer.Serialize(new { productoId = productId }),
            cancellationToken);

        return result;
    }

    public async Task RemoveCodeAsync(Guid productId, Guid codeId, CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productId"] = new[] { "El producto es obligatorio." }
                });
        }

        if (codeId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["codeId"] = new[] { "El codigo es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var removed = await _productRepository.RemoveCodeAsync(tenantId, productId, codeId, cancellationToken);
        if (removed is null)
        {
            throw new NotFoundException("Codigo no encontrado.");
        }

        await _auditLogService.LogAsync(
            "ProductoCodigo",
            codeId.ToString(),
            AuditAction.Delete,
            JsonSerializer.Serialize(removed),
            null,
            JsonSerializer.Serialize(new { productoId = productId }),
            cancellationToken);
    }

    public async Task DeleteAsync(Guid productId, CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productId"] = new[] { "El producto es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var before = await _productRepository.GetByIdAsync(tenantId, productId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        var deleted = await _productRepository.DeleteAsync(tenantId, productId, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        await _auditLogService.LogAsync(
            "Producto",
            productId.ToString(),
            AuditAction.Delete,
            JsonSerializer.Serialize(before),
            null,
            null,
            cancellationToken);
    }

    public async Task<ProductProveedorDto> AddProveedorAsync(
        Guid productId,
        ProductProveedorCreateDto request,
        CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productId"] = new[] { "El producto es obligatorio." }
                });
        }

        if (request is null)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["request"] = new[] { "El request es obligatorio." }
                });
        }

        if (request.ProveedorId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var now = DateTimeOffset.UtcNow;
        var esPrincipal = request.EsPrincipal ?? false;

        var result = await _productRepository.AddProveedorAsync(
            tenantId,
            productId,
            request.ProveedorId,
            esPrincipal,
            now,
            cancellationToken);

        if (result is null)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        await _auditLogService.LogAsync(
            "ProductoProveedor",
            result.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(result),
            JsonSerializer.Serialize(new { productoId = productId }),
            cancellationToken);

        return result;
    }

    public async Task<ProductProveedorDto> SetProveedorPrincipalAsync(
        Guid productId,
        Guid relationId,
        ProductProveedorUpdateDto request,
        CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productId"] = new[] { "El producto es obligatorio." }
                });
        }

        if (relationId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["relationId"] = new[] { "La relacion es obligatoria." }
                });
        }

        if (request is null || !request.EsPrincipal)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["esPrincipal"] = new[] { "Debe marcar como principal." }
                });
        }

        var tenantId = EnsureTenant();
        var result = await _productRepository.SetProveedorPrincipalAsync(
            tenantId,
            productId,
            relationId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        if (result is null)
        {
            throw new NotFoundException("Proveedor no encontrado para el producto.");
        }

        await _auditLogService.LogAsync(
            "ProductoProveedor",
            result.Id.ToString(),
            AuditAction.Update,
            null,
            JsonSerializer.Serialize(result),
            JsonSerializer.Serialize(new { productoId = productId }),
            cancellationToken);

        return result;
    }

    private Guid EnsureTenant()
    {
        if (_requestContext.TenantId == Guid.Empty)
        {
            throw new UnauthorizedException("Contexto de tenant invalido.");
        }

        return _requestContext.TenantId;
    }
}
