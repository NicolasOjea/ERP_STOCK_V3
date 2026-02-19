using Pos.Application.Abstractions;
using Pos.Application.DTOs.Etiquetas;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Etiquetas;

public sealed class EtiquetasService
{
    private const string ListaPrecioMinorista = "Minorista";

    private readonly IProductRepository _productRepository;
    private readonly IEtiquetaPdfGenerator _pdfGenerator;
    private readonly ICodigoBarraPdfGenerator _codigoBarraPdfGenerator;
    private readonly IRequestContext _requestContext;

    public EtiquetasService(
        IProductRepository productRepository,
        IEtiquetaPdfGenerator pdfGenerator,
        ICodigoBarraPdfGenerator codigoBarraPdfGenerator,
        IRequestContext requestContext)
    {
        _productRepository = productRepository;
        _pdfGenerator = pdfGenerator;
        _codigoBarraPdfGenerator = codigoBarraPdfGenerator;
        _requestContext = requestContext;
    }

    public async Task<byte[]> GenerarPdfAsync(EtiquetaRequestDto request, CancellationToken cancellationToken)
    {
        if (request is null || request.ProductoIds is null || request.ProductoIds.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productoIds"] = new[] { "Debe incluir al menos un producto." }
                });
        }

        if (request.ProductoIds.Any(id => id == Guid.Empty))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productoIds"] = new[] { "Producto invalido en la lista." }
                });
        }

        var tenantId = EnsureTenant();
        var ids = request.ProductoIds.Distinct().ToList();

        var items = await _productRepository.GetLabelDataAsync(
            tenantId,
            ids,
            ListaPrecioMinorista,
            cancellationToken);

        if (items.Count != ids.Count)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        var data = new EtiquetaPdfDataDto(items);
        return _pdfGenerator.Generate(data);
    }

    public async Task<byte[]> GenerarCodigosBarraPdfAsync(
        CodigosBarraRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request is null || request.ProductoIds is null || request.ProductoIds.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productoIds"] = new[] { "Debe incluir al menos un producto." }
                });
        }

        if (request.ProductoIds.Any(id => id == Guid.Empty))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productoIds"] = new[] { "Producto invalido en la lista." }
                });
        }

        var tenantId = EnsureTenant();
        var ids = request.ProductoIds.Distinct().ToList();

        var productos = await _productRepository.GetBarcodeDataAsync(tenantId, ids, cancellationToken);
        if (productos.Count != ids.Count)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        var proveedores = productos
            .GroupBy(
                p => string.IsNullOrWhiteSpace(p.ProveedorNombre) ? "SIN PROVEEDOR" : p.ProveedorNombre!.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var items = g.OrderBy(i => i.Nombre, StringComparer.OrdinalIgnoreCase)
                    .Select(i => new CodigoBarraItemPdfDto(i.Nombre, i.Sku))
                    .ToList();
                return new CodigoBarraProveedorPdfDto(g.Key, items);
            })
            .ToList();

        var data = new CodigoBarraPdfDataDto(proveedores);
        return _codigoBarraPdfGenerator.Generate(data);
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
