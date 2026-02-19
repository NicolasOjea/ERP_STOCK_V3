using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Proveedores;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Proveedores;

public sealed class ProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public ProveedorService(
        IProveedorRepository proveedorRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _proveedorRepository = proveedorRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<IReadOnlyList<ProveedorDto>> SearchAsync(
        string? search,
        bool? activo,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        return await _proveedorRepository.SearchAsync(tenantId, search, activo, cancellationToken);
    }

    public async Task<ProveedorDto> CreateAsync(ProveedorCreateDto request, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(request.Telefono))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["telefono"] = new[] { "El telefono es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var now = DateTimeOffset.UtcNow;
        var normalized = request with
        {
            Name = request.Name.Trim(),
            Telefono = request.Telefono.Trim(),
            Cuit = string.IsNullOrWhiteSpace(request.Cuit) ? null : request.Cuit.Trim(),
            Direccion = string.IsNullOrWhiteSpace(request.Direccion) ? null : request.Direccion.Trim()
        };

        var id = await _proveedorRepository.CreateAsync(tenantId, normalized, now, cancellationToken);
        var proveedor = await _proveedorRepository.GetByIdAsync(tenantId, id, cancellationToken);
        if (proveedor is null)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        await _auditLogService.LogAsync(
            "Proveedor",
            proveedor.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(proveedor),
            null,
            cancellationToken);

        return proveedor;
    }

    public async Task<ProveedorDto> UpdateAsync(Guid proveedorId, ProveedorUpdateDto request, CancellationToken cancellationToken)
    {
        if (proveedorId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor es obligatorio." }
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

        var hasChange = request.Name is not null
            || request.Telefono is not null
            || request.Cuit is not null
            || request.Direccion is not null
            || request.IsActive is not null;
        if (!hasChange)
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

        if (request.Telefono is not null && string.IsNullOrWhiteSpace(request.Telefono))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["telefono"] = new[] { "El telefono no puede estar vacio." }
                });
        }

        var tenantId = EnsureTenant();
        var before = await _proveedorRepository.GetByIdAsync(tenantId, proveedorId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        var normalized = request with
        {
            Name = request.Name?.Trim(),
            Telefono = request.Telefono?.Trim(),
            Cuit = request.Cuit is null ? null : string.IsNullOrWhiteSpace(request.Cuit) ? null : request.Cuit.Trim(),
            Direccion = request.Direccion is null ? null : string.IsNullOrWhiteSpace(request.Direccion) ? null : request.Direccion.Trim()
        };
        var updated = await _proveedorRepository.UpdateAsync(tenantId, proveedorId, normalized, DateTimeOffset.UtcNow, cancellationToken);
        if (!updated)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        var after = await _proveedorRepository.GetByIdAsync(tenantId, proveedorId, cancellationToken);
        if (after is null)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        await _auditLogService.LogAsync(
            "Proveedor",
            proveedorId.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(after),
            null,
            cancellationToken);

        return after;
    }

    public async Task DeleteAsync(Guid proveedorId, CancellationToken cancellationToken)
    {
        if (proveedorId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var before = await _proveedorRepository.GetByIdAsync(tenantId, proveedorId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        var deleted = await _proveedorRepository.DeleteAsync(tenantId, proveedorId, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        await _auditLogService.LogAsync(
            "Proveedor",
            proveedorId.ToString(),
            AuditAction.Delete,
            JsonSerializer.Serialize(before),
            null,
            null,
            cancellationToken);
    }

    public async Task<ProveedorDeletePreviewDto> GetDeletePreviewAsync(
        Guid proveedorId,
        CancellationToken cancellationToken)
    {
        if (proveedorId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var proveedor = await _proveedorRepository.GetByIdAsync(tenantId, proveedorId, cancellationToken);
        if (proveedor is null)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        var products = await _proveedorRepository.GetDeleteProductOptionsAsync(tenantId, proveedorId, cancellationToken);
        return new ProveedorDeletePreviewDto(proveedorId, proveedor.Name, products);
    }

    public async Task DeleteWithProductResolutionAsync(
        Guid proveedorId,
        ProveedorDeleteRequestDto request,
        CancellationToken cancellationToken)
    {
        if (proveedorId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["proveedorId"] = new[] { "El proveedor es obligatorio." }
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

        var tenantId = EnsureTenant();
        var before = await _proveedorRepository.GetByIdAsync(tenantId, proveedorId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        var deleted = await _proveedorRepository.DeleteWithProductResolutionAsync(
            tenantId,
            proveedorId,
            request.ProductIdsToDelete ?? Array.Empty<Guid>(),
            DateTimeOffset.UtcNow,
            cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException("Proveedor no encontrado.");
        }

        await _auditLogService.LogAsync(
            "Proveedor",
            proveedorId.ToString(),
            AuditAction.Delete,
            JsonSerializer.Serialize(before),
            null,
            JsonSerializer.Serialize(new
            {
                deletedProductIds = request.ProductIdsToDelete
            }),
            cancellationToken);
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
