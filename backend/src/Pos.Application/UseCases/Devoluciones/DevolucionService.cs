using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Devoluciones;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Devoluciones;

public sealed class DevolucionService
{
    private readonly IDevolucionRepository _devolucionRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public DevolucionService(
        IDevolucionRepository devolucionRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _devolucionRepository = devolucionRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<DevolucionResultDto> CrearAsync(
        DevolucionCreateDto request,
        CancellationToken cancellationToken)
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

        if (request.VentaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ventaId"] = new[] { "La venta es obligatoria." }
                });
        }

        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["motivo"] = new[] { "El motivo es obligatorio." }
                });
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["items"] = new[] { "Debe incluir items para devolver." }
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
                        ["items"] = new[] { "Producto invalido en items." }
                    });
            }

            if (item.Cantidad <= 0)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["items"] = new[] { "La cantidad debe ser mayor a 0." }
                    });
            }
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        var userId = EnsureUser();

        var normalized = request with
        {
            Motivo = request.Motivo.Trim()
        };

        var result = await _devolucionRepository.CreateAsync(
            tenantId,
            sucursalId,
            userId,
            normalized,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "Devolucion",
            result.Devolucion.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(result.Devolucion),
            JsonSerializer.Serialize(new { ventaId = normalized.VentaId }),
            cancellationToken);

        await _auditLogService.LogAsync(
            "NotaCreditoInterna",
            result.NotaCredito.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(result.NotaCredito),
            JsonSerializer.Serialize(new { devolucionId = result.Devolucion.Id }),
            cancellationToken);

        foreach (var cambio in result.StockCambios)
        {
            await _auditLogService.LogAsync(
                "StockSaldo",
                $"{cambio.ProductoId}:{sucursalId}",
                AuditAction.Adjust,
                JsonSerializer.Serialize(new { cantidadActual = cambio.SaldoAntes }),
                JsonSerializer.Serialize(new { cantidadActual = cambio.SaldoDespues }),
                JsonSerializer.Serialize(new { movimientoId = cambio.MovimientoId, itemId = cambio.MovimientoItemId, devolucionId = result.Devolucion.Id }),
                cancellationToken);
        }

        foreach (var movimiento in result.CajaMovimientos)
        {
            await _auditLogService.LogAsync(
                "CajaMovimiento",
                movimiento.Movimiento.Id.ToString(),
                AuditAction.Adjust,
                JsonSerializer.Serialize(new { saldo = movimiento.SaldoAntes }),
                JsonSerializer.Serialize(new { saldo = movimiento.SaldoDespues }),
                JsonSerializer.Serialize(new { devolucionId = result.Devolucion.Id }),
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

    private Guid EnsureUser()
    {
        if (_requestContext.UserId == Guid.Empty)
        {
            throw new UnauthorizedException("Contexto de usuario invalido.");
        }

        return _requestContext.UserId;
    }
}
