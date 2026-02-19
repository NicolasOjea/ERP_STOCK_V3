using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.DocumentosCompra;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.DocumentosCompra;

public sealed class DocumentoCompraService
{
    private readonly IDocumentParser _documentParser;
    private readonly IDocumentoCompraRepository _documentoCompraRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public DocumentoCompraService(
        IDocumentParser documentParser,
        IDocumentoCompraRepository documentoCompraRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _documentParser = documentParser;
        _documentoCompraRepository = documentoCompraRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<DocumentoCompraParseResultDto> ParseAsync(JsonElement input, CancellationToken cancellationToken)
    {
        var parsed = _documentParser.Parse(input);

        if (string.IsNullOrWhiteSpace(parsed.Numero))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["numero"] = new[] { "El numero es obligatorio." }
                });
        }

        if (parsed.Items is null || parsed.Items.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["items"] = new[] { "Debe incluir items." }
                });
        }

        foreach (var item in parsed.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Codigo))
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["sku"] = new[] { "El SKU es obligatorio." }
                    });
            }

            if (string.IsNullOrWhiteSpace(item.Descripcion))
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["descripcion"] = new[] { "La descripcion es obligatoria." }
                    });
            }

            if (item.Cantidad <= 0)
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

        var normalized = parsed with
        {
            Numero = parsed.Numero.Trim(),
            Items = parsed.Items
                .Select(i => new ParsedDocumentItemDto(
                    i.Codigo.Trim(),
                    i.Descripcion.Trim(),
                    i.Cantidad,
                    i.CostoUnitario))
                .ToList()
        };

        var documentoId = await _documentoCompraRepository.CreateAsync(
            tenantId,
            sucursalId,
            normalized,
            DateTimeOffset.UtcNow,
            cancellationToken);

        var documento = await _documentoCompraRepository.GetByIdAsync(tenantId, sucursalId, documentoId, cancellationToken);
        if (documento is null)
        {
            throw new NotFoundException("Documento de compra no encontrado.");
        }

        await _auditLogService.LogAsync(
            "DocumentoCompra",
            documento.Id.ToString(),
            AuditAction.Create,
            null,
            JsonSerializer.Serialize(documento),
            null,
            cancellationToken);

        return new DocumentoCompraParseResultDto(
            documento.Id,
            documento.Items);
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
