using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Caja;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Caja;

public sealed class CajaService
{
    private readonly ICajaRepository _cajaRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public CajaService(
        ICajaRepository cajaRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _cajaRepository = cajaRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<IReadOnlyList<CajaDto>> GetCajasAsync(
        bool? activo,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        return await _cajaRepository.GetCajasAsync(tenantId, sucursalId, activo, cancellationToken);
    }

    public async Task<CajaDto> CreateCajaAsync(CajaCreateDto request, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["nombre"] = new[] { "El nombre es obligatorio." }
                });
        }

        if (string.IsNullOrWhiteSpace(request.Numero))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["numero"] = new[] { "El numero es obligatorio." }
                });
        }

        if (!request.Numero.All(char.IsDigit))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["numero"] = new[] { "El numero debe contener solo digitos." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        var now = DateTimeOffset.UtcNow;

        var normalized = request with
        {
            Nombre = request.Nombre.Trim(),
            Numero = request.Numero.Trim()
        };

        var created = await _cajaRepository.CreateCajaAsync(tenantId, sucursalId, normalized, now, cancellationToken);

        await _auditLogService.LogAsync(
            "Caja",
            created.Id.ToString(),
            AuditAction.Create,
            null,
            System.Text.Json.JsonSerializer.Serialize(created),
            null,
            cancellationToken);

        return created;
    }

    public async Task<CajaSesionDto> AbrirSesionAsync(CajaSesionAbrirDto request, CancellationToken cancellationToken)
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

        if (request.CajaId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["cajaId"] = new[] { "La caja es obligatoria." }
                });
        }

        if (request.MontoInicial < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["montoInicial"] = new[] { "El monto inicial debe ser mayor o igual a 0." }
                });
        }

        if (string.IsNullOrWhiteSpace(request.Turno))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["turno"] = new[] { "El turno es obligatorio." }
                });
        }

        var turno = request.Turno.Trim().ToUpperInvariant();
        var turnosValidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "MANANA", "TARDE", "NOCHE" };
        if (!turnosValidos.Contains(turno))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["turno"] = new[] { "Turno invalido. Usa MANANA, TARDE o NOCHE." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var exists = await _cajaRepository.CajaExistsAsync(tenantId, sucursalId, request.CajaId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Caja no encontrada.");
        }

        var hasOpen = await _cajaRepository.HasOpenSessionAsync(tenantId, request.CajaId, cancellationToken);
        if (hasOpen)
        {
            throw new ConflictException("Ya existe una sesion abierta para esta caja.");
        }

        var session = await _cajaRepository.OpenSessionAsync(
            tenantId,
            sucursalId,
            request.CajaId,
            request.MontoInicial,
            turno,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "CajaSesion",
            session.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(session),
            null,
            cancellationToken);

        return session;
    }

    public async Task<CajaMovimientoDto> RegistrarMovimientoAsync(
        Guid sesionId,
        CajaMovimientoCreateDto request,
        CancellationToken cancellationToken)
    {
        if (sesionId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["sesionId"] = new[] { "La sesion es obligatoria." }
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

        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["motivo"] = new[] { "El motivo es obligatorio." }
                });
        }

        if (!Enum.TryParse<CajaMovimientoTipo>(request.Tipo, true, out var tipo))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["tipo"] = new[] { "Tipo de movimiento invalido." }
                });
        }

        if (request.Monto == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["monto"] = new[] { "El monto no puede ser 0." }
                });
        }

        decimal montoSigned;
        switch (tipo)
        {
            case CajaMovimientoTipo.Retiro:
            case CajaMovimientoTipo.Gasto:
                if (request.Monto <= 0)
                {
                    throw new ValidationException(
                        "Validacion fallida.",
                        new Dictionary<string, string[]>
                        {
                            ["monto"] = new[] { "El monto debe ser mayor a 0." }
                        });
                }
                montoSigned = -Math.Abs(request.Monto);
                break;
            case CajaMovimientoTipo.Ajuste:
                montoSigned = request.Monto;
                break;
            default:
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["tipo"] = new[] { "Tipo de movimiento invalido." }
                    });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        var medioPago = string.IsNullOrWhiteSpace(request.MedioPago)
            ? "EFECTIVO"
            : request.MedioPago.Trim().ToUpperInvariant();

        var result = await _cajaRepository.AddMovimientoAsync(
            tenantId,
            sucursalId,
            sesionId,
            tipo,
            montoSigned,
            request.Motivo.Trim(),
            medioPago,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "CajaMovimiento",
            result.Movimiento.Id.ToString(),
            AuditAction.Adjust,
            JsonSerializer.Serialize(new { saldo = result.SaldoAntes }),
            JsonSerializer.Serialize(new { saldo = result.SaldoDespues }),
            JsonSerializer.Serialize(new { cajaSesionId = sesionId }),
            cancellationToken);

        return result.Movimiento;
    }

    public async Task<CajaCierreResultDto> CerrarSesionAsync(
        Guid sesionId,
        CajaCierreRequestDto request,
        CancellationToken cancellationToken)
    {
        if (sesionId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["sesionId"] = new[] { "La sesion es obligatoria." }
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

        if (request.EfectivoContado < 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["efectivoContado"] = new[] { "El efectivo contado no puede ser negativo." }
                });
        }

        var medios = request.Medios ?? Array.Empty<CajaCierreMedioDto>();
        foreach (var medio in medios)
        {
            if (string.IsNullOrWhiteSpace(medio.Medio))
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["medios"] = new[] { "El medio de pago es obligatorio." }
                    });
            }

            if (medio.Contado < 0)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["medios"] = new[] { "El monto contado no puede ser negativo." }
                    });
            }

            if (string.Equals(medio.Medio.Trim(), "EFECTIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["medios"] = new[] { "El efectivo se informa en el campo efectivoContado." }
                    });
            }
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var before = await _cajaRepository.GetSesionAsync(tenantId, sucursalId, sesionId, cancellationToken);
        if (before is null)
        {
            throw new NotFoundException("Sesion no encontrada.");
        }

        var normalized = request with
        {
            Medios = medios
                .Select(m => new CajaCierreMedioDto(m.Medio.Trim().ToUpperInvariant(), m.Contado))
                .ToArray()
        };

        var result = await _cajaRepository.CloseSessionAsync(
            tenantId,
            sucursalId,
            sesionId,
            normalized,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "CajaSesion",
            sesionId.ToString(),
            AuditAction.Close,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(result),
            JsonSerializer.Serialize(new { sesionId }),
            cancellationToken);

        return result;
    }

    public async Task<CajaResumenDto> GetResumenAsync(Guid sesionId, CancellationToken cancellationToken)
    {
        if (sesionId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["sesionId"] = new[] { "La sesion es obligatoria." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        var resumen = await _cajaRepository.GetResumenAsync(tenantId, sucursalId, sesionId, cancellationToken);
        if (resumen is null)
        {
            throw new NotFoundException("Sesion no encontrada.");
        }

        return resumen;
    }

    public async Task<IReadOnlyList<CajaHistorialDto>> GetHistorialAsync(
        DateOnly? from,
        DateOnly? to,
        CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        // El filtro de fecha en UI es fecha local (Argentina). Convertimos ese rango local a UTC.
        var appOffset = TimeSpan.FromHours(-3);
        DateTimeOffset? fromUtc = from.HasValue
            ? new DateTimeOffset(from.Value.ToDateTime(TimeOnly.MinValue), appOffset).ToUniversalTime()
            : null;
        DateTimeOffset? toUtc = to.HasValue
            ? new DateTimeOffset(to.Value.ToDateTime(TimeOnly.MaxValue), appOffset).ToUniversalTime()
            : null;

        if (fromUtc.HasValue && toUtc.HasValue && fromUtc > toUtc)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["fecha"] = new[] { "Rango de fechas invalido." }
                });
        }

        return await _cajaRepository.GetSesionesHistoricasAsync(tenantId, sucursalId, fromUtc, toUtc, cancellationToken);
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
