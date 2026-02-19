using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Compras;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Compras;

public sealed class OrdenCompraService
{
    private readonly IOrdenCompraRepository _ordenCompraRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public OrdenCompraService(
        IOrdenCompraRepository ordenCompraRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _ordenCompraRepository = ordenCompraRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<OrdenCompraDto> CreateAsync(OrdenCompraCreateDto request, CancellationToken cancellationToken)
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

        if (request.ProveedorId.HasValue && request.ProveedorId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor es invalido." }
                });
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["items"] = new[] { "Debe incluir al menos un item." }
                });
        }

        var duplicates = request.Items
            .GroupBy(i => i.ProductoId)
            .Where(g => g.Key != Guid.Empty && g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["items"] = new[] { "No se permiten productos duplicados." }
                });
        }

        foreach (var item in request.Items)
        {
            if (item.ProductoId == Guid.Empty)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["productoId"] = new[] { "El producto es obligatorio." }
                    });
            }

            if (item.Cantidad <= 0)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["cantidad"] = new[] { "La cantidad debe ser mayor a 0." }
                    });
            }
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var ordenId = await _ordenCompraRepository.CreateAsync(
            tenantId,
            sucursalId,
            request,
            DateTimeOffset.UtcNow,
            cancellationToken);

        var orden = await _ordenCompraRepository.GetByIdAsync(tenantId, sucursalId, ordenId, cancellationToken);
        if (orden is null)
        {
            throw new NotFoundException("Orden de compra no encontrada.");
        }

        await _auditLogService.LogAsync(
            "OrdenCompra",
            orden.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(orden),
            null,
            cancellationToken);

        return orden;
    }

    public async Task<IReadOnlyList<OrdenCompraListItemDto>> GetListAsync(CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        return await _ordenCompraRepository.GetListAsync(tenantId, sucursalId, cancellationToken);
    }

    public async Task<OrdenCompraDto> GetByIdAsync(Guid ordenCompraId, CancellationToken cancellationToken)
    {
        if (ordenCompraId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ordenCompraId"] = new[] { "La orden es obligatoria." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var orden = await _ordenCompraRepository.GetByIdAsync(tenantId, sucursalId, ordenCompraId, cancellationToken);
        if (orden is null)
        {
            throw new NotFoundException("Orden de compra no encontrada.");
        }

        return orden;
    }

    public async Task<OrdenCompraDto> EnviarAsync(Guid ordenCompraId, CancellationToken cancellationToken)
    {
        if (ordenCompraId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ordenCompraId"] = new[] { "La orden es obligatoria." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var before = await _ordenCompraRepository.GetByIdAsync(tenantId, sucursalId, ordenCompraId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Orden de compra no encontrada.");
        }

        var after = await _ordenCompraRepository.EnviarAsync(
            tenantId,
            sucursalId,
            ordenCompraId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "OrdenCompra",
            ordenCompraId.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(after),
            null,
            cancellationToken);

        return after;
    }

    public async Task<OrdenCompraDto> CancelarAsync(Guid ordenCompraId, CancellationToken cancellationToken)
    {
        if (ordenCompraId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ordenCompraId"] = new[] { "La orden es obligatoria." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var before = await _ordenCompraRepository.GetByIdAsync(tenantId, sucursalId, ordenCompraId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Orden de compra no encontrada.");
        }

        var after = await _ordenCompraRepository.CancelarAsync(
            tenantId,
            sucursalId,
            ordenCompraId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "OrdenCompra",
            ordenCompraId.ToString(),
            AuditAction.Cancel,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(after),
            null,
            cancellationToken);

        return after;
    }

    private Guid EnsureTenant()
    {
        if (_requestContext.TenantId == Guid.Empty)
        {
            throw new UnauthorizedException("Contexto de tenant invalido.");
        }

        return _requestContext.TenantId;
    }

    private Guid EnsureSucursal()
    {
        if (_requestContext.SucursalId == Guid.Empty)
        {
            throw new UnauthorizedException("Contexto de sucursal invalido.");
        }

        return _requestContext.SucursalId;
    }
}
