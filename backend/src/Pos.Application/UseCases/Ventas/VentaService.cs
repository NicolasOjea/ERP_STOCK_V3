using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Ventas;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Ventas;

public sealed class VentaService
{
    private const string ListaPrecioDefault = "Minorista";

    private readonly IVentaRepository _ventaRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public VentaService(
        IVentaRepository ventaRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _ventaRepository = ventaRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<VentaDto> IniciarVentaAsync(CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        var userId = EnsureUser();

        var ventaId = await _ventaRepository.CreateAsync(
            tenantId,
            sucursalId,
            userId,
            ListaPrecioDefault,
            DateTimeOffset.UtcNow,
            cancellationToken);

        var venta = await _ventaRepository.GetByIdAsync(tenantId, sucursalId, ventaId, cancellationToken);
        if (venta is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        await _auditLogService.LogAsync(
            "Venta",
            venta.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(venta),
            null,
            cancellationToken);

        return venta;
    }

    public async Task<VentaItemDto> AgregarItemPorCodigoAsync(
        Guid ventaId,
        VentaScanRequestDto request,
        CancellationToken cancellationToken)
    {
        if (ventaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ventaId"] = new[] { "La venta es obligatoria." }
                });
        }

        if (request is null || string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["code"] = new[] { "El SKU es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        var code = request.Code.Trim();

        var change = await _ventaRepository.AddItemByCodeAsync(
            tenantId,
            sucursalId,
            ventaId,
            code,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "VentaItem",
            change.Item.Id.ToString(),
            change.Creado ? AuditAction.Create : AuditAction.Update,
            JsonSerializer.Serialize(new { cantidad = change.CantidadAntes }),
            JsonSerializer.Serialize(new { cantidad = change.CantidadDespues }),
            JsonSerializer.Serialize(new { ventaId }),
            cancellationToken);

        return change.Item;
    }

    public async Task<VentaItemDto> AgregarItemPorProductoAsync(
        Guid ventaId,
        VentaItemByProductRequestDto request,
        CancellationToken cancellationToken)
    {
        if (ventaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ventaId"] = new[] { "La venta es obligatoria." }
                });
        }

        if (request is null || request.ProductId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productId"] = new[] { "El producto es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var change = await _ventaRepository.AddItemByProductAsync(
            tenantId,
            sucursalId,
            ventaId,
            request.ProductId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "VentaItem",
            change.Item.Id.ToString(),
            change.Creado ? AuditAction.Create : AuditAction.Update,
            JsonSerializer.Serialize(new { cantidad = change.CantidadAntes }),
            JsonSerializer.Serialize(new { cantidad = change.CantidadDespues }),
            JsonSerializer.Serialize(new { ventaId }),
            cancellationToken);

        return change.Item;
    }

    public async Task<VentaItemDto> ActualizarItemAsync(
        Guid ventaId,
        Guid itemId,
        VentaItemUpdateDto request,
        CancellationToken cancellationToken)
    {
        if (ventaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ventaId"] = new[] { "La venta es obligatoria." }
                });
        }

        if (itemId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["itemId"] = new[] { "El item es obligatorio." }
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

        if (request.Cantidad < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["cantidad"] = new[] { "La cantidad debe ser mayor a 0." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        if (request.Cantidad == 0)
        {
            var removed = await _ventaRepository.RemoveItemAsync(
                tenantId,
                sucursalId,
                ventaId,
                itemId,
                DateTimeOffset.UtcNow,
                cancellationToken);

            await _auditLogService.LogAsync(
                "VentaItem",
                removed.Id.ToString(),
                AuditAction.Delete,
                JsonSerializer.Serialize(new { cantidad = removed.Cantidad }),
                null,
                JsonSerializer.Serialize(new { ventaId }),
                cancellationToken);

            return removed;
        }

        var change = await _ventaRepository.UpdateItemCantidadAsync(
            tenantId,
            sucursalId,
            ventaId,
            itemId,
            request.Cantidad,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "VentaItem",
            change.Item.Id.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(new { cantidad = change.CantidadAntes }),
            JsonSerializer.Serialize(new { cantidad = change.CantidadDespues }),
            JsonSerializer.Serialize(new { ventaId }),
            cancellationToken);

        return change.Item;
    }

    public async Task<VentaItemDto> QuitarItemAsync(
        Guid ventaId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        if (ventaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ventaId"] = new[] { "La venta es obligatoria." }
                });
        }

        if (itemId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["itemId"] = new[] { "El item es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var removed = await _ventaRepository.RemoveItemAsync(
            tenantId,
            sucursalId,
            ventaId,
            itemId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "VentaItem",
            removed.Id.ToString(),
            AuditAction.Delete,
            JsonSerializer.Serialize(new { cantidad = removed.Cantidad }),
            null,
            JsonSerializer.Serialize(new { ventaId }),
            cancellationToken);

        return removed;
    }

    public async Task<VentaDto> GetByIdAsync(Guid ventaId, CancellationToken cancellationToken)
    {
        if (ventaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ventaId"] = new[] { "La venta es obligatoria." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var venta = await _ventaRepository.GetByIdAsync(tenantId, sucursalId, ventaId, cancellationToken);
        if (venta is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        return venta;
    }

    public async Task<VentaTicketDto> GetTicketByNumeroAsync(long numeroVenta, CancellationToken cancellationToken)
    {
        if (numeroVenta <= 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["numeroVenta"] = new[] { "El numero de venta es invalido." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var ticket = await _ventaRepository.GetTicketByNumeroAsync(tenantId, sucursalId, numeroVenta, cancellationToken);
        if (ticket is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        return ticket;
    }

    public async Task<VentaConfirmResultDto> ConfirmarVentaAsync(
        Guid ventaId,
        VentaConfirmRequestDto request,
        CancellationToken cancellationToken)
    {
        if (ventaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ventaId"] = new[] { "La venta es obligatoria." }
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

        if (request.CajaSesionId.HasValue && request.CajaSesionId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["cajaSesionId"] = new[] { "La sesion de caja es invalida." }
                });
        }

        if (!request.Facturada.HasValue)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["facturada"] = new[] { "Debes indicar si la venta es facturada o no facturada." }
                });
        }

        var pagos = request.Pagos ?? Array.Empty<VentaPagoRequestDto>();
        foreach (var pago in pagos)
        {
            if (string.IsNullOrWhiteSpace(pago.MedioPago))
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["pagos"] = new[] { "El medio de pago es obligatorio." }
                    });
            }

            if (pago.Monto <= 0)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["pagos"] = new[] { "El monto debe ser mayor a 0." }
                    });
            }
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var before = await _ventaRepository.GetByIdAsync(tenantId, sucursalId, ventaId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        var normalized = request with
        {
            Pagos = pagos
                .Select(p => new VentaPagoRequestDto(p.MedioPago.Trim().ToUpperInvariant(), p.Monto))
                .ToArray()
        };

        var result = await _ventaRepository.ConfirmAsync(
            tenantId,
            sucursalId,
            ventaId,
            normalized,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "Venta",
            ventaId.ToString(),
            AuditAction.Confirm,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(result.Venta),
            null,
            cancellationToken);

        foreach (var cambio in result.StockCambios)
        {
            await _auditLogService.LogAsync(
                "StockSaldo",
                $"{cambio.ProductoId}:{sucursalId}",
                AuditAction.Adjust,
                JsonSerializer.Serialize(new { cantidadActual = cambio.SaldoAntes }),
                JsonSerializer.Serialize(new { cantidadActual = cambio.SaldoDespues }),
                JsonSerializer.Serialize(new { movimientoId = cambio.MovimientoId, itemId = cambio.MovimientoItemId, ventaId }),
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
                JsonSerializer.Serialize(new { ventaId }),
                cancellationToken);
        }

        return result;
    }

    public async Task<VentaAnularResultDto> AnularVentaAsync(
        Guid ventaId,
        VentaAnularRequestDto request,
        CancellationToken cancellationToken)
    {
        if (ventaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["ventaId"] = new[] { "La venta es obligatoria." }
                });
        }

        if (request is null || string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["motivo"] = new[] { "El motivo es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var before = await _ventaRepository.GetByIdAsync(tenantId, sucursalId, ventaId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        var normalized = request with { Motivo = request.Motivo.Trim() };

        var result = await _ventaRepository.AnularAsync(
            tenantId,
            sucursalId,
            ventaId,
            normalized,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "Venta",
            ventaId.ToString(),
            AuditAction.Cancel,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(result.Venta),
            JsonSerializer.Serialize(new { motivo = normalized.Motivo }),
            cancellationToken);

        foreach (var cambio in result.StockCambios)
        {
            await _auditLogService.LogAsync(
                "StockSaldo",
                $"{cambio.ProductoId}:{sucursalId}",
                AuditAction.Adjust,
                JsonSerializer.Serialize(new { cantidadActual = cambio.SaldoAntes }),
                JsonSerializer.Serialize(new { cantidadActual = cambio.SaldoDespues }),
                JsonSerializer.Serialize(new { movimientoId = cambio.MovimientoId, itemId = cambio.MovimientoItemId, ventaId }),
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
                JsonSerializer.Serialize(new { ventaId }),
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
