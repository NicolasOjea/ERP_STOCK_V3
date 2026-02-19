using Pos.Application.Abstractions;
using Pos.Application.DTOs.Auditoria;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Auditoria;

public sealed class AuditoriaService
{
    private const int DefaultPage = 1;
    private const int DefaultSize = 50;
    private const int MaxSize = 200;

    private readonly IAuditLogQueryRepository _repository;
    private readonly IRequestContext _requestContext;

    public AuditoriaService(
        IAuditLogQueryRepository repository,
        IRequestContext requestContext)
    {
        _repository = repository;
        _requestContext = requestContext;
    }

    public async Task<AuditLogQueryResultDto> SearchAsync(
        string? entidad,
        Guid? usuarioId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        int? page,
        int? size,
        CancellationToken cancellationToken)
    {
        var finalPage = page ?? DefaultPage;
        var finalSize = size ?? DefaultSize;

        if (finalPage <= 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["page"] = new[] { "La pagina debe ser mayor a 0." }
                });
        }

        if (finalSize <= 0 || finalSize > MaxSize)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["size"] = new[] { $"El tamaño debe estar entre 1 y {MaxSize}." }
                });
        }

        if (usuarioId.HasValue && usuarioId.Value == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["usuarioId"] = new[] { "El usuario es invalido." }
                });
        }

        if (desde.HasValue && hasta.HasValue && desde > hasta)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["fecha"] = new[] { "El rango de fechas es invalido." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var request = new AuditLogQueryRequestDto(
            string.IsNullOrWhiteSpace(entidad) ? null : entidad.Trim(),
            usuarioId,
            desde,
            hasta,
            finalPage,
            finalSize);

        return await _repository.SearchAsync(tenantId, sucursalId, request, cancellationToken);
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
