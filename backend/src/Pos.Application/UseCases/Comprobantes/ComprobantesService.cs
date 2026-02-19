using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Comprobantes;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Comprobantes;

public sealed class ComprobantesService
{
    private readonly IComprobanteRepository _repository;
    private readonly IFiscalProvider _fiscalProvider;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public ComprobantesService(
        IComprobanteRepository repository,
        IFiscalProvider fiscalProvider,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _repository = repository;
        _fiscalProvider = fiscalProvider;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<ComprobanteDto> CrearBorradorDesdeVentaAsync(
        Guid ventaId,
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

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();
        var userId = EnsureUser();

        var comprobante = await _repository.CreateBorradorAsync(
            tenantId,
            sucursalId,
            userId,
            ventaId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "Comprobante",
            comprobante.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(comprobante),
            JsonSerializer.Serialize(new { ventaId }),
            cancellationToken);

        return comprobante;
    }

    public async Task<ComprobanteDto> EmitirAsync(
        Guid comprobanteId,
        CancellationToken cancellationToken)
    {
        if (comprobanteId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["id"] = new[] { "El comprobante es obligatorio." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var comprobante = await _repository.GetByIdAsync(tenantId, sucursalId, comprobanteId, cancellationToken);
        if (comprobante is null)
        {
            throw new NotFoundException("Comprobante no encontrado.");
        }

        if (!string.Equals(comprobante.Estado, ComprobanteEstado.Borrador.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflictException("El comprobante no esta en borrador.");
        }

        var emitRequest = new FiscalEmitRequestDto(
            comprobante.Id,
            comprobante.VentaId,
            comprobante.Total,
            DateTimeOffset.UtcNow);

        var result = await _fiscalProvider.EmitirAsync(emitRequest, cancellationToken);

        var updated = await _repository.EmitirAsync(
            tenantId,
            sucursalId,
            comprobanteId,
            result,
            DateTimeOffset.UtcNow,
            cancellationToken);

        await _auditLogService.LogAsync(
            "Comprobante",
            updated.Id.ToString(),
            AuditAction.Update,
            JsonSerializer.Serialize(comprobante),
            JsonSerializer.Serialize(updated),
            JsonSerializer.Serialize(new { provider = result.Provider }),
            cancellationToken);

        return updated;
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
