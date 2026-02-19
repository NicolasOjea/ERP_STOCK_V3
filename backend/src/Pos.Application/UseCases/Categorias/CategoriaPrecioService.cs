using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Categorias;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Categorias;

public sealed class CategoriaPrecioService
{
    private readonly ICategoriaPrecioRepository _categoriaRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public CategoriaPrecioService(
        ICategoriaPrecioRepository categoriaRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _categoriaRepository = categoriaRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<IReadOnlyList<CategoriaPrecioDto>> SearchAsync(
        string? search,
        bool? activo,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        return await _categoriaRepository.SearchAsync(tenantId, search, activo, cancellationToken);
    }

    public async Task<CategoriaPrecioDto> CreateAsync(CategoriaPrecioCreateDto request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["name"] = new[] { "El nombre es obligatorio." }
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
        var normalized = request with { Name = request.Name.Trim() };
        var id = await _categoriaRepository.CreateAsync(tenantId, normalized, DateTimeOffset.UtcNow, cancellationToken);
        var created = await _categoriaRepository.GetByIdAsync(tenantId, id, cancellationToken);
        if (created is null)
        {
            throw new NotFoundException("Categoria no encontrada.");
        }

        await _auditLogService.LogAsync(
            "Categoria",
            created.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(created),
            null,
            cancellationToken);

        return created;
    }

    public async Task<CategoriaPrecioDto> UpdateAsync(
        Guid categoriaId,
        CategoriaPrecioUpdateDto request,
        CancellationToken cancellationToken)
    {
        if (categoriaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["categoriaId"] = new[] { "La categoria es obligatoria." }
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
            || request.MargenGananciaPct is not null
            || request.IsActive is not null
            || request.AplicarAProductos is not null;

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
        var before = await _categoriaRepository.GetByIdAsync(tenantId, categoriaId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Categoria no encontrada.");
        }

        var normalized = request with { Name = request.Name?.Trim() };
        var updated = await _categoriaRepository.UpdateAsync(tenantId, categoriaId, normalized, DateTimeOffset.UtcNow, cancellationToken);
        if (!updated)
        {
            throw new NotFoundException("Categoria no encontrada.");
        }

        var after = await _categoriaRepository.GetByIdAsync(tenantId, categoriaId, cancellationToken);
        if (after is null)
        {
            throw new NotFoundException("Categoria no encontrada.");
        }

        await _auditLogService.LogAsync(
            "Categoria",
            categoriaId.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(after),
            JsonSerializer.Serialize(new { aplicarAProductos = request.AplicarAProductos ?? true }),
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
}
