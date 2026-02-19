using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Empresa;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Empresa;

public sealed class EmpresaDatosService
{
    private readonly IEmpresaDatosRepository _empresaDatosRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public EmpresaDatosService(
        IEmpresaDatosRepository empresaDatosRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _empresaDatosRepository = empresaDatosRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<EmpresaDatosDto> GetAsync(CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        var data = await _empresaDatosRepository.GetAsync(tenantId, cancellationToken);
        if (data is not null)
        {
            return data;
        }

        return new EmpresaDatosDto(Guid.Empty, string.Empty, null, null, null, null, null, null);
    }

    public async Task<EmpresaDatosDto> UpsertAsync(EmpresaDatosUpsertDto request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.RazonSocial))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["razonSocial"] = new[] { "La razon social es obligatoria." }
                });
        }

        var tenantId = EnsureTenant();
        var before = await _empresaDatosRepository.GetAsync(tenantId, cancellationToken);
        var normalized = request with
        {
            RazonSocial = request.RazonSocial.Trim(),
            Cuit = NormalizeNullable(request.Cuit),
            Telefono = NormalizeNullable(request.Telefono),
            Direccion = NormalizeNullable(request.Direccion),
            Email = NormalizeNullable(request.Email),
            Web = NormalizeNullable(request.Web),
            Observaciones = NormalizeNullable(request.Observaciones)
        };

        var saved = await _empresaDatosRepository.UpsertAsync(tenantId, normalized, DateTimeOffset.UtcNow, cancellationToken);

        await _auditLogService.LogAsync(
            "EmpresaDatos",
            saved.Id == Guid.Empty ? tenantId.ToString() : saved.Id.ToString(),
            before is null ? AuditAction.Create : AuditAction.Update,
            JsonSerializer.Serialize(before),
            JsonSerializer.Serialize(saved),
            null,
            cancellationToken);

        return saved;
    }

    private static string? NormalizeNullable(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var trimmed = value.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
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
