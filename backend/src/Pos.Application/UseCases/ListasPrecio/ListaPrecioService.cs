using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.ListasPrecio;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.ListasPrecio;

public sealed class ListaPrecioService
{
    private readonly IListaPrecioRepository _listaPrecioRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public ListaPrecioService(
        IListaPrecioRepository listaPrecioRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _listaPrecioRepository = listaPrecioRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<IReadOnlyList<ListaPrecioDto>> GetListAsync(CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        return await _listaPrecioRepository.GetListAsync(tenantId, cancellationToken);
    }

    public async Task<ListaPrecioDto> CreateAsync(ListaPrecioCreateDto request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Nombre))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["nombre"] = new[] { "El nombre es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var normalized = request with { Nombre = request.Nombre.Trim() };
        var id = await _listaPrecioRepository.CreateAsync(tenantId, normalized, DateTimeOffset.UtcNow, cancellationToken);
        var created = await _listaPrecioRepository.GetByIdAsync(tenantId, id, cancellationToken);
        if (created is null)
        {
            throw new NotFoundException("Lista de precio no encontrada.");
        }

        await _auditLogService.LogAsync(
            "ListaPrecio",
            created.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(created),
            null,
            cancellationToken);

        return created;
    }

    public async Task<ListaPrecioDto> UpdateAsync(Guid listaPrecioId, ListaPrecioUpdateDto request, CancellationToken cancellationToken)
    {
        if (listaPrecioId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["listaPrecioId"] = new[] { "La lista es obligatoria." }
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

        var hasChange = request.Nombre is not null || request.IsActive is not null;
        if (!hasChange)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["request"] = new[] { "Debe enviar al menos un cambio." }
                });
        }

        if (request.Nombre is not null && string.IsNullOrWhiteSpace(request.Nombre))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["nombre"] = new[] { "El nombre no puede estar vacio." }
                });
        }

        var tenantId = EnsureTenant();
        var before = await _listaPrecioRepository.GetByIdAsync(tenantId, listaPrecioId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Lista de precio no encontrada.");
        }

        var normalized = request with { Nombre = request.Nombre?.Trim() };
        var updated = await _listaPrecioRepository.UpdateAsync(tenantId, listaPrecioId, normalized, DateTimeOffset.UtcNow, cancellationToken);
        if (!updated)
        {
            throw new NotFoundException("Lista de precio no encontrada.");
        }

        var after = await _listaPrecioRepository.GetByIdAsync(tenantId, listaPrecioId, cancellationToken);
        if (after is null)
        {
            throw new NotFoundException("Lista de precio no encontrada.");
        }

        await _auditLogService.LogAsync(
            "ListaPrecio",
            listaPrecioId.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(after),
            null,
            cancellationToken);

        return after;
    }

    public async Task UpsertItemsAsync(Guid listaPrecioId, ListaPrecioItemsUpdateDto request, CancellationToken cancellationToken)
    {
        if (listaPrecioId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["listaPrecioId"] = new[] { "La lista es obligatoria." }
                });
        }

        if (request is null || request.Items is null || request.Items.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["items"] = new[] { "Debe incluir items." }
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

            if (item.Precio <= 0)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["precio"] = new[] { "El precio debe ser mayor a 0." }
                    });
            }
        }

        var tenantId = EnsureTenant();
        var before = await _listaPrecioRepository.GetByIdAsync(tenantId, listaPrecioId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Lista de precio no encontrada.");
        }

        await _listaPrecioRepository.UpsertItemsAsync(
            tenantId,
            listaPrecioId,
            request.Items,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "ListaPrecioItems",
            listaPrecioId.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(before),
            null,
            JsonSerializer.Serialize(new { totalItems = request.Items.Count }),
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
