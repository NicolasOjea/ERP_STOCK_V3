using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.PreRecepciones;
using Pos.Application.DTOs.Recepciones;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.PreRecepciones;

public sealed class PreRecepcionService
{
    private readonly IPreRecepcionRepository _preRecepcionRepository;
    private readonly IRecepcionRepository _recepcionRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public PreRecepcionService(
        IPreRecepcionRepository preRecepcionRepository,
        IRecepcionRepository recepcionRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _preRecepcionRepository = preRecepcionRepository;
        _recepcionRepository = recepcionRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<PreRecepcionDto> CreateAsync(PreRecepcionCreateDto request, CancellationToken cancellationToken)
    {
        if (request is null || request.DocumentoCompraId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["documentoCompraId"] = new[] { "El documento es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var preRecepcionId = await _preRecepcionRepository.CreateAsync(
            tenantId,
            sucursalId,
            request.DocumentoCompraId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        var preRecepcion = await _preRecepcionRepository.GetByIdAsync(tenantId, sucursalId, preRecepcionId, cancellationToken);
        if (preRecepcion is null)
        {
            throw new NotFoundException("Pre-recepcion no encontrada.");
        }

        await _auditLogService.LogAsync(
            "PreRecepcion",
            preRecepcion.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(preRecepcion),
            null,
            cancellationToken);

        return preRecepcion;
    }

    public async Task<PreRecepcionDto> GetByIdAsync(Guid preRecepcionId, CancellationToken cancellationToken)
    {
        if (preRecepcionId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["preRecepcionId"] = new[] { "La pre-recepcion es obligatoria." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var preRecepcion = await _preRecepcionRepository.GetByIdAsync(tenantId, sucursalId, preRecepcionId, cancellationToken);
        if (preRecepcion is null)
        {
            throw new NotFoundException("Pre-recepcion no encontrada.");
        }

        return preRecepcion;
    }

    public async Task<PreRecepcionDto> UpdateAsync(
        Guid preRecepcionId,
        PreRecepcionUpdateDto request,
        CancellationToken cancellationToken)
    {
        if (preRecepcionId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["preRecepcionId"] = new[] { "La pre-recepcion es obligatoria." }
                });
        }

        if (request is null || request.Items is null || request.Items.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["items"] = new[] { "Debe incluir items para actualizar." }
                });
        }

        foreach (var item in request.Items)
        {
            if (item.ItemId == Guid.Empty)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["itemId"] = new[] { "El item es obligatorio." }
                    });
            }

            if (item.ProductoId.HasValue && item.ProductoId.Value == Guid.Empty)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["productoId"] = new[] { "El producto es invalido." }
                    });
            }

            if (item.Cantidad.HasValue && item.Cantidad.Value <= 0)
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

        var before = await _preRecepcionRepository.GetByIdAsync(tenantId, sucursalId, preRecepcionId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Pre-recepcion no encontrada.");
        }

        var updated = await _preRecepcionRepository.UpdateAsync(
            tenantId,
            sucursalId,
            preRecepcionId,
            request,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "PreRecepcion",
            preRecepcionId.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(updated),
            null,
            cancellationToken);

        return updated;
    }

    public async Task<RecepcionConfirmResultDto> ConfirmarAsync(
        Guid preRecepcionId,
        CancellationToken cancellationToken)
    {
        if (preRecepcionId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["preRecepcionId"] = new[] { "La pre-recepcion es obligatoria." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var result = await _recepcionRepository.ConfirmarAsync(
            tenantId,
            sucursalId,
            preRecepcionId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "Recepcion",
            result.Recepcion.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(result.Recepcion),
            JsonSerializer.Serialize(new { preRecepcionId }),
            cancellationToken);

        foreach (var cambio in result.StockCambios)
        {
            await _auditLogService.LogAsync(
                "StockSaldo",
                $"{cambio.ProductoId}:{sucursalId}",
                AuditAction.Adjust,
                JsonSerializer.Serialize(new { cantidadActual = cambio.SaldoAntes }),
                JsonSerializer.Serialize(new { cantidadActual = cambio.SaldoDespues }),
                JsonSerializer.Serialize(new { movimientoId = cambio.MovimientoId, itemId = cambio.MovimientoItemId, preRecepcionId }),
                cancellationToken);
        }

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

    private Guid EnsureSucursal()
    {
        if (_requestContext.SucursalId == Guid.Empty)
        {
            throw new UnauthorizedException("Contexto de sucursal invalido.");
        }

        return _requestContext.SucursalId;
    }
}
