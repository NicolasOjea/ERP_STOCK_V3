using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Etiquetas;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly PosDbContext _dbContext;

    public ProductRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductListItemDto>> SearchAsync(
        Guid tenantId,
        string? search,
        Guid? categoriaId,
        bool? activo,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId);

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (activo.HasValue)
        {
            query = query.Where(p => p.IsActive == activo.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, $"%{term}%")
                || EF.Functions.ILike(p.Sku, $"%{term}%"));
        }

        var results = await (from p in query
                join c in _dbContext.Categorias.AsNoTracking().Where(c => c.TenantId == tenantId)
                    on p.CategoriaId equals c.Id into cat
                from c in cat.DefaultIfEmpty()
                join m in _dbContext.Marcas.AsNoTracking().Where(m => m.TenantId == tenantId)
                    on p.MarcaId equals m.Id into mar
                from m in mar.DefaultIfEmpty()
                join pr in _dbContext.Proveedores.AsNoTracking().Where(pr => pr.TenantId == tenantId)
                    on p.ProveedorId equals pr.Id into prov
                from pr in prov.DefaultIfEmpty()
                orderby p.Name
                select new
                {
                    p.Id,
                    p.Name,
                    p.Sku,
                    p.CategoriaId,
                    Categoria = c != null ? c.Name : null,
                    p.MarcaId,
                    Marca = m != null ? m.Name : null,
                    p.ProveedorId,
                    Proveedor = pr != null ? pr.Name : null,
                    p.PrecioBase,
                    p.PrecioVenta,
                    p.PricingMode,
                    p.MargenGananciaPct,
                    p.IsActive
                })
            .ToListAsync(cancellationToken);

        if (results.Count == 0)
        {
            return Array.Empty<ProductListItemDto>();
        }

        var list = results.Select(r =>
        {
            var codigoFinal = r.Sku;
            return new ProductListItemDto(
                r.Id,
                r.Name,
                r.Sku,
                codigoFinal,
                r.CategoriaId,
                r.Categoria,
                r.MarcaId,
                r.Marca,
                r.ProveedorId,
                r.Proveedor,
                r.PrecioBase,
                r.PrecioVenta,
                r.PricingMode.ToString().ToUpperInvariant(),
                r.MargenGananciaPct,
                r.IsActive);
        }).ToList();

        return list;
    }

    public async Task<ProductDetailDto?> GetByIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await (from p in _dbContext.Productos.AsNoTracking()
                where p.TenantId == tenantId && p.Id == productId
                join c in _dbContext.Categorias.AsNoTracking().Where(c => c.TenantId == tenantId)
                    on p.CategoriaId equals c.Id into cat
                from c in cat.DefaultIfEmpty()
                join m in _dbContext.Marcas.AsNoTracking().Where(m => m.TenantId == tenantId)
                    on p.MarcaId equals m.Id into mar
                from m in mar.DefaultIfEmpty()
                join pr in _dbContext.Proveedores.AsNoTracking().Where(pr => pr.TenantId == tenantId)
                    on p.ProveedorId equals pr.Id into prov
                from pr in prov.DefaultIfEmpty()
                select new
                {
                    Product = p,
                    Categoria = c != null ? c.Name : null,
                    Marca = m != null ? m.Name : null,
                    Proveedor = pr != null ? pr.Name : null
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return null;
        }

        var codes = await _dbContext.ProductoCodigos.AsNoTracking()
            .Where(c => c.TenantId == tenantId && c.ProductoId == productId)
            .OrderBy(c => c.Codigo)
            .Select(c => new ProductCodeDto(c.Id, c.Codigo))
            .ToListAsync(cancellationToken);

        return new ProductDetailDto(
            product.Product.Id,
            product.Product.Name,
            product.Product.Sku,
            product.Product.CategoriaId,
            product.Categoria,
            product.Product.MarcaId,
            product.Marca,
            product.Product.ProveedorId,
            product.Proveedor,
            product.Product.PrecioBase,
            product.Product.PrecioVenta,
            product.Product.PricingMode.ToString().ToUpperInvariant(),
            product.Product.MargenGananciaPct,
            product.Product.IsActive,
            codes);
    }

    public async Task<Guid> CreateAsync(
        Guid tenantId,
        ProductCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var skuExists = await _dbContext.Productos.AsNoTracking()
            .AnyAsync(p => p.TenantId == tenantId && p.Sku == request.Sku, cancellationToken);

        if (skuExists)
        {
            throw new ConflictException("SKU ya existe.");
        }

        var codeExists = await _dbContext.ProductoCodigos.AsNoTracking()
            .AnyAsync(c => c.TenantId == tenantId && c.Codigo == request.Sku, cancellationToken);

        if (codeExists)
        {
            throw new ConflictException("SKU ya existe.");
        }

        if (request.CategoriaId.HasValue)
        {
            var categoriaExists = await _dbContext.Categorias.AsNoTracking()
                .AnyAsync(c => c.TenantId == tenantId && c.Id == request.CategoriaId.Value, cancellationToken);

            if (!categoriaExists)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["categoriaId"] = new[] { "La categoria no existe." }
                    });
            }
        }

        if (request.MarcaId.HasValue)
        {
            var marcaExists = await _dbContext.Marcas.AsNoTracking()
                .AnyAsync(m => m.TenantId == tenantId && m.Id == request.MarcaId.Value, cancellationToken);

            if (!marcaExists)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["marcaId"] = new[] { "La marca no existe." }
                    });
            }
        }

        if (request.ProveedorId.HasValue)
        {
            var proveedorExists = await _dbContext.Proveedores.AsNoTracking()
                .AnyAsync(p => p.TenantId == tenantId && p.Id == request.ProveedorId.Value, cancellationToken);

            if (!proveedorExists)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["proveedorId"] = new[] { "El proveedor no existe." }
                    });
            }
        }

        var precioBase = request.PrecioBase ?? 1m;
        var pricingMode = ParsePricingMode(request.PricingMode, ProductPricingMode.FijoPct);
        var pricing = await ResolvePricingAsync(
            tenantId,
            request.CategoriaId,
            pricingMode,
            request.MargenGananciaPct,
            precioBase,
            request.PrecioVenta,
            cancellationToken);

        var product = new Producto(
            Guid.NewGuid(),
            tenantId,
            request.Name,
            request.Sku,
            request.CategoriaId,
            request.MarcaId,
            request.ProveedorId,
            nowUtc,
            precioBase,
            pricing.PrecioVenta,
            pricingMode,
            pricing.MargenGananciaPct,
            request.IsActive ?? true);

        _dbContext.Productos.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await EnsureSkuCodeAsync(tenantId, product.Id, request.Sku, nowUtc, cancellationToken);

        if (request.ProveedorId.HasValue)
        {
            var relation = new ProductoProveedor(
                Guid.NewGuid(),
                tenantId,
                product.Id,
                request.ProveedorId.Value,
                true,
                nowUtc);
            _dbContext.ProductoProveedores.Add(relation);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return product.Id;
    }

    public async Task<bool> UpdateAsync(
        Guid tenantId,
        Guid productId,
        ProductUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Productos
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == productId, cancellationToken);

        if (product is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.Sku) && !string.Equals(request.Sku, product.Sku, StringComparison.OrdinalIgnoreCase))
        {
            var skuExists = await _dbContext.Productos.AsNoTracking()
                .AnyAsync(p => p.TenantId == tenantId && p.Sku == request.Sku, cancellationToken);

            if (skuExists)
            {
                throw new ConflictException("SKU ya existe.");
            }

            var codeExists = await _dbContext.ProductoCodigos.AsNoTracking()
                .AnyAsync(c => c.TenantId == tenantId && c.Codigo == request.Sku && c.ProductoId != productId, cancellationToken);

            if (codeExists)
            {
                throw new ConflictException("SKU ya existe.");
            }
        }

        if (request.CategoriaId.HasValue)
        {
            var categoriaExists = await _dbContext.Categorias.AsNoTracking()
                .AnyAsync(c => c.TenantId == tenantId && c.Id == request.CategoriaId.Value, cancellationToken);

            if (!categoriaExists)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["categoriaId"] = new[] { "La categoria no existe." }
                    });
            }
        }

        if (request.MarcaId.HasValue)
        {
            var marcaExists = await _dbContext.Marcas.AsNoTracking()
                .AnyAsync(m => m.TenantId == tenantId && m.Id == request.MarcaId.Value, cancellationToken);

            if (!marcaExists)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["marcaId"] = new[] { "La marca no existe." }
                    });
            }
        }

        if (request.ProveedorId.HasValue)
        {
            var proveedorExists = await _dbContext.Proveedores.AsNoTracking()
                .AnyAsync(p => p.TenantId == tenantId && p.Id == request.ProveedorId.Value, cancellationToken);

            if (!proveedorExists)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["proveedorId"] = new[] { "El proveedor no existe." }
                    });
            }
        }

        var newName = string.IsNullOrWhiteSpace(request.Name) ? product.Name : request.Name;
        var newSku = string.IsNullOrWhiteSpace(request.Sku) ? product.Sku : request.Sku;
        var newCategoriaId = request.CategoriaId ?? product.CategoriaId;
        var newMarcaId = request.MarcaId ?? product.MarcaId;
        var newProveedorId = request.ProveedorId ?? product.ProveedorId;
        var newIsActive = request.IsActive ?? product.IsActive;
        var newPrecioBase = request.PrecioBase ?? product.PrecioBase;
        var newPricingMode = ParsePricingMode(request.PricingMode, product.PricingMode);
        var requestedMargin = request.MargenGananciaPct.HasValue ? request.MargenGananciaPct : product.MargenGananciaPct;
        var requestedPrecioVenta = request.PrecioVenta.HasValue ? request.PrecioVenta : product.PrecioVenta;

        var pricing = await ResolvePricingAsync(
            tenantId,
            newCategoriaId,
            newPricingMode,
            requestedMargin,
            newPrecioBase,
            requestedPrecioVenta,
            cancellationToken);

        product.Update(
            newName,
            newSku,
            newCategoriaId,
            newMarcaId,
            newProveedorId,
            newPrecioBase,
            pricing.PrecioVenta,
            newPricingMode,
            pricing.MargenGananciaPct,
            newIsActive,
            nowUtc);

        if (request.ProveedorId.HasValue)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var relations = await _dbContext.ProductoProveedores
                .Where(r => r.TenantId == tenantId && r.ProductoId == productId)
                .ToListAsync(cancellationToken);

            var relation = relations.FirstOrDefault(r => r.ProveedorId == request.ProveedorId.Value);
            if (relation is null)
            {
                relation = new ProductoProveedor(
                    Guid.NewGuid(),
                    tenantId,
                    productId,
                    request.ProveedorId.Value,
                    true,
                    nowUtc);
                _dbContext.ProductoProveedores.Add(relation);
            }

            foreach (var rel in relations.Where(r => r.EsPrincipal && r.Id != relation.Id))
            {
                rel.SetPrincipal(false, nowUtc);
            }

            if (!relation.EsPrincipal)
            {
                relation.SetPrincipal(true, nowUtc);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await EnsureSkuCodeAsync(tenantId, productId, newSku, nowUtc, cancellationToken);
            return true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await EnsureSkuCodeAsync(tenantId, productId, newSku, nowUtc, cancellationToken);
        return true;
    }

    public async Task<ProductCodeDto?> AddCodeAsync(
        Guid tenantId,
        Guid productId,
        string code,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var productExists = await _dbContext.Productos.AsNoTracking()
            .AnyAsync(p => p.TenantId == tenantId && p.Id == productId, cancellationToken);

        if (!productExists)
        {
            return null;
        }

        var codeExists = await _dbContext.ProductoCodigos.AsNoTracking()
            .AnyAsync(c => c.TenantId == tenantId && c.Codigo == code, cancellationToken);

        if (codeExists)
        {
            throw new ConflictException("Codigo ya existe");
        }

        var entity = new ProductoCodigo(Guid.NewGuid(), tenantId, productId, code, nowUtc);
        _dbContext.ProductoCodigos.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProductCodeDto(entity.Id, entity.Codigo);
    }

    public async Task<ProductCodeDto?> RemoveCodeAsync(
        Guid tenantId,
        Guid productId,
        Guid codeId,
        CancellationToken cancellationToken = default)
    {
        var code = await _dbContext.ProductoCodigos
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.ProductoId == productId && c.Id == codeId, cancellationToken);

        if (code is null)
        {
            return null;
        }

        var dto = new ProductCodeDto(code.Id, code.Codigo);
        _dbContext.ProductoCodigos.Remove(code);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return dto;
    }

    public async Task<bool> DeleteAsync(
        Guid tenantId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Productos
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == productId, cancellationToken);

        if (product is null)
        {
            return false;
        }

        var hasUsage =
            await _dbContext.VentaItems.AsNoTracking().AnyAsync(i => i.TenantId == tenantId && i.ProductoId == productId, cancellationToken)
            || await _dbContext.StockMovimientoItems.AsNoTracking().AnyAsync(i => i.TenantId == tenantId && i.ProductoId == productId, cancellationToken)
            || await _dbContext.RecepcionItems.AsNoTracking().AnyAsync(i => i.TenantId == tenantId && i.ProductoId == productId, cancellationToken)
            || await _dbContext.DevolucionItems.AsNoTracking().AnyAsync(i => i.TenantId == tenantId && i.ProductoId == productId, cancellationToken)
            || await _dbContext.OrdenCompraItems.AsNoTracking().AnyAsync(i => i.TenantId == tenantId && i.ProductoId == productId, cancellationToken)
            || await _dbContext.PreRecepcionItems.AsNoTracking().AnyAsync(i => i.TenantId == tenantId && i.ProductoId == productId, cancellationToken);

        if (hasUsage)
        {
            throw new ConflictException("No se puede eliminar el producto porque tiene ventas o movimientos asociados.");
        }

        var codigos = await _dbContext.ProductoCodigos
            .Where(c => c.TenantId == tenantId && c.ProductoId == productId)
            .ToListAsync(cancellationToken);
        var relacionesProveedor = await _dbContext.ProductoProveedores
            .Where(r => r.TenantId == tenantId && r.ProductoId == productId)
            .ToListAsync(cancellationToken);
        var configuracionesStock = await _dbContext.ProductoStockConfigs
            .Where(c => c.TenantId == tenantId && c.ProductoId == productId)
            .ToListAsync(cancellationToken);
        var saldosStock = await _dbContext.StockSaldos
            .Where(s => s.TenantId == tenantId && s.ProductoId == productId)
            .ToListAsync(cancellationToken);
        var listaPrecioItems = await _dbContext.ListaPrecioItems
            .Where(i => i.TenantId == tenantId && i.ProductoId == productId)
            .ToListAsync(cancellationToken);

        _dbContext.ProductoCodigos.RemoveRange(codigos);
        _dbContext.ProductoProveedores.RemoveRange(relacionesProveedor);
        _dbContext.ProductoStockConfigs.RemoveRange(configuracionesStock);
        _dbContext.StockSaldos.RemoveRange(saldosStock);
        _dbContext.ListaPrecioItems.RemoveRange(listaPrecioItems);
        _dbContext.Productos.Remove(product);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ProductProveedorDto?> AddProveedorAsync(
        Guid tenantId,
        Guid productId,
        Guid proveedorId,
        bool esPrincipal,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var product = await _dbContext.Productos
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == productId, cancellationToken);

        if (product is null)
        {
            return null;
        }

        var proveedorExists = await _dbContext.Proveedores.AsNoTracking()
            .AnyAsync(p => p.TenantId == tenantId && p.Id == proveedorId, cancellationToken);

        if (!proveedorExists)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor no existe." }
                });
        }

        var relations = await _dbContext.ProductoProveedores
            .Where(r => r.TenantId == tenantId && r.ProductoId == productId)
            .ToListAsync(cancellationToken);

        var relation = relations.FirstOrDefault(r => r.ProveedorId == proveedorId);
        if (relation is null)
        {
            relation = new ProductoProveedor(Guid.NewGuid(), tenantId, productId, proveedorId, esPrincipal, nowUtc);
            _dbContext.ProductoProveedores.Add(relation);
        }

        if (esPrincipal)
        {
            foreach (var rel in relations.Where(r => r.EsPrincipal && r.Id != relation.Id))
            {
                rel.SetPrincipal(false, nowUtc);
            }

            if (!relation.EsPrincipal)
            {
                relation.SetPrincipal(true, nowUtc);
            }

            product.SetProveedorPrincipal(proveedorId, nowUtc);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var proveedor = await _dbContext.Proveedores.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.Id == proveedorId)
            .Select(p => new { p.Id, p.Name })
            .FirstOrDefaultAsync(cancellationToken);

        var nombre = proveedor?.Name ?? string.Empty;
        return new ProductProveedorDto(relation.Id, proveedorId, nombre, relation.EsPrincipal);
    }

    public async Task<ProductProveedorDto?> SetProveedorPrincipalAsync(
        Guid tenantId,
        Guid productId,
        Guid relationId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var product = await _dbContext.Productos
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == productId, cancellationToken);

        if (product is null)
        {
            return null;
        }

        var relations = await _dbContext.ProductoProveedores
            .Where(r => r.TenantId == tenantId && r.ProductoId == productId)
            .ToListAsync(cancellationToken);

        var relation = relations.FirstOrDefault(r => r.Id == relationId);
        if (relation is null)
        {
            return null;
        }

        foreach (var rel in relations.Where(r => r.EsPrincipal && r.Id != relation.Id))
        {
            rel.SetPrincipal(false, nowUtc);
        }

        if (!relation.EsPrincipal)
        {
            relation.SetPrincipal(true, nowUtc);
        }

        product.SetProveedorPrincipal(relation.ProveedorId, nowUtc);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var proveedor = await _dbContext.Proveedores.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.Id == relation.ProveedorId)
            .Select(p => new { p.Id, p.Name })
            .FirstOrDefaultAsync(cancellationToken);

        var nombre = proveedor?.Name ?? string.Empty;
        return new ProductProveedorDto(relation.Id, relation.ProveedorId, nombre, relation.EsPrincipal);
    }

    public async Task<Guid?> GetIdBySkuAsync(
        Guid tenantId,
        string sku,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return null;
        }

        var normalized = sku.Trim();
        return await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.Sku == normalized)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid?> GetIdByCodeAsync(
        Guid tenantId,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var normalized = code.Trim();
        return await _dbContext.ProductoCodigos.AsNoTracking()
            .Where(c => c.TenantId == tenantId && c.Codigo == normalized)
            .Select(c => (Guid?)c.ProductoId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EtiquetaItemDto>> GetLabelDataAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> productIds,
        string listaPrecio,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return Array.Empty<EtiquetaItemDto>();
        }

        var products = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && ids.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Sku,
                p.PrecioBase,
                p.PrecioVenta
            })
            .ToListAsync(cancellationToken);

        Dictionary<Guid, decimal> prices = new();
        if (!string.IsNullOrWhiteSpace(listaPrecio))
        {
            var lista = await _dbContext.ListasPrecio.AsNoTracking()
                .FirstOrDefaultAsync(l => l.TenantId == tenantId && l.Nombre == listaPrecio, cancellationToken);

            if (lista is not null)
            {
                prices = await _dbContext.ListaPrecioItems.AsNoTracking()
                    .Where(i => i.TenantId == tenantId && i.ListaPrecioId == lista.Id && ids.Contains(i.ProductoId))
                    .ToDictionaryAsync(i => i.ProductoId, i => i.Precio, cancellationToken);
            }
        }

        var result = products
            .OrderBy(p => p.Name)
            .Select(p =>
            {
                var codigo = p.Sku;
                var precio = prices.TryGetValue(p.Id, out var price) ? price : (p.PrecioVenta > 0 ? p.PrecioVenta : p.PrecioBase);
                return new EtiquetaItemDto(p.Id, p.Name, precio, codigo);
            })
            .ToList();

        return result;
    }

    public async Task<IReadOnlyList<CodigoBarraProductoDto>> GetBarcodeDataAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return Array.Empty<CodigoBarraProductoDto>();
        }

        var rows = await (from p in _dbContext.Productos.AsNoTracking()
                where p.TenantId == tenantId && ids.Contains(p.Id)
                join pr in _dbContext.Proveedores.AsNoTracking().Where(pr => pr.TenantId == tenantId)
                    on p.ProveedorId equals pr.Id into prov
                from pr in prov.DefaultIfEmpty()
                select new CodigoBarraProductoDto(
                    p.Id,
                    p.Name,
                    p.Sku,
                    p.ProveedorId,
                    pr != null ? pr.Name : null))
            .ToListAsync(cancellationToken);

        return rows;
    }

    private static ProductPricingMode ParsePricingMode(string? value, ProductPricingMode fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        return value.Trim().ToUpperInvariant() switch
        {
            "MANUAL" => ProductPricingMode.Manual,
            "CATEGORIA" => ProductPricingMode.Categoria,
            _ => ProductPricingMode.FijoPct
        };
    }

    private async Task<(decimal PrecioVenta, decimal? MargenGananciaPct)> ResolvePricingAsync(
        Guid tenantId,
        Guid? categoriaId,
        ProductPricingMode pricingMode,
        decimal? margenGananciaPct,
        decimal precioBase,
        decimal? precioVenta,
        CancellationToken cancellationToken)
    {
        if (precioBase < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["precioBase"] = new[] { "El precio base no puede ser negativo." }
                });
        }

        if (pricingMode == ProductPricingMode.Manual)
        {
            var manualPrice = precioVenta ?? 0m;
            if (manualPrice < 0)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["precioVenta"] = new[] { "El precio manual no puede ser negativo." }
                    });
            }

            return (manualPrice, null);
        }

        if (pricingMode == ProductPricingMode.Categoria)
        {
            if (!categoriaId.HasValue)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["categoriaId"] = new[] { "La categoria es obligatoria para precio por categoria." }
                    });
            }

            var categoria = await _dbContext.Categorias.AsNoTracking()
                .Where(c => c.TenantId == tenantId && c.Id == categoriaId.Value)
                .Select(c => new { c.MargenGananciaPct })
                .FirstOrDefaultAsync(cancellationToken);

            if (categoria is null)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["categoriaId"] = new[] { "La categoria no existe." }
                    });
            }

            var calculated = CalculateSalePrice(precioBase, categoria.MargenGananciaPct);
            return (calculated, categoria.MargenGananciaPct);
        }

        var margin = margenGananciaPct ?? 30m;
        if (margin < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["margenGananciaPct"] = new[] { "El margen no puede ser negativo." }
                });
        }

        return (CalculateSalePrice(precioBase, margin), margin);
    }

    private static decimal CalculateSalePrice(decimal precioBase, decimal margenPct)
    {
        var factor = 1m + (margenPct / 100m);
        var value = precioBase * factor;
        return decimal.Round(value, 4, MidpointRounding.AwayFromZero);
    }

    private async Task EnsureSkuCodeAsync(
        Guid tenantId,
        Guid productId,
        string sku,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken)
    {
        var normalized = sku?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return;
        }

        var existsForProduct = await _dbContext.ProductoCodigos.AsNoTracking()
            .AnyAsync(c => c.TenantId == tenantId && c.ProductoId == productId && c.Codigo == normalized, cancellationToken);

        if (existsForProduct)
        {
            return;
        }

        var entity = new ProductoCodigo(Guid.NewGuid(), tenantId, productId, normalized, nowUtc);
        _dbContext.ProductoCodigos.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

