using System.Globalization;
using System.Text;
using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Importaciones;
using Pos.Application.DTOs.Products;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Importaciones;

public sealed class ImportacionProductosService
{
    private readonly IProductRepository _productRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public ImportacionProductosService(
        IProductRepository productRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _productRepository = productRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public ProductImportPreviewDto Preview(string csvContent)
    {
        var parsed = ParseCsv(csvContent);
        if (parsed.Errors.Count > 0)
        {
            throw new ValidationException("Validacion fallida.", parsed.Errors);
        }

        var rows = parsed.Rows
            .Select(row => new ProductImportRowDto(
                row.RowNumber,
                row.Sku,
                row.Nombre,
                row.Codigo,
                row.PrecioBase,
                row.Activo,
                row.Errors,
                row.Errors.Count == 0))
            .ToList();

        var validCount = rows.Count(r => r.IsValid);
        var errorCount = rows.Count - validCount;

        return new ProductImportPreviewDto(rows, validCount, errorCount);
    }

    public async Task<ProductImportConfirmResultDto> ConfirmAsync(
        string csvContent,
        CancellationToken cancellationToken)
    {
        var parsed = ParseCsv(csvContent);
        if (parsed.Errors.Count > 0)
        {
            throw new ValidationException("Validacion fallida.", parsed.Errors);
        }

        var invalidRows = parsed.Rows.Where(r => r.Errors.Count > 0).ToList();
        if (invalidRows.Count > 0)
        {
            var errors = invalidRows.ToDictionary(
                r => $"row-{r.RowNumber}",
                r => r.Errors.ToArray());
            throw new ValidationException("Validacion fallida.", errors);
        }

        var tenantId = EnsureTenant();
        var now = DateTimeOffset.UtcNow;

        var created = 0;
        var updated = 0;

        foreach (var row in parsed.Rows)
        {
            var sku = row.Sku?.Trim();
            var codigo = row.Codigo?.Trim();
            var nombre = row.Nombre?.Trim();

            var idBySku = !string.IsNullOrWhiteSpace(sku)
                ? await _productRepository.GetIdBySkuAsync(tenantId, sku, cancellationToken)
                : null;
            var idByCode = !string.IsNullOrWhiteSpace(codigo)
                ? await _productRepository.GetIdByCodeAsync(tenantId, codigo, cancellationToken)
                : null;

            if (idBySku.HasValue && idByCode.HasValue && idBySku.Value != idByCode.Value)
            {
                throw new ConflictException("SKU y codigo pertenecen a productos distintos.");
            }

            var productId = idBySku ?? idByCode;
            if (!productId.HasValue)
            {
                if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ValidationException(
                        "Validacion fallida.",
                        new Dictionary<string, string[]>
                        {
                            [$"row-{row.RowNumber}"] = new[] { "SKU y nombre son obligatorios para crear." }
                        });
                }

                var request = new ProductCreateDto(
                    nombre,
                    sku,
                    null,
                    null,
                    null,
                    row.Activo,
                    row.PrecioBase);

                var newId = await _productRepository.CreateAsync(tenantId, request, now, cancellationToken);

                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    await _productRepository.AddCodeAsync(tenantId, newId, codigo, now, cancellationToken);
                }

                var createdProduct = await _productRepository.GetByIdAsync(tenantId, newId, cancellationToken);
                if (createdProduct is null)
                {
                    throw new NotFoundException("Producto no encontrado.");
                }

                await _auditLogService.LogAsync(
                    "Producto",
                    createdProduct.Id.ToString(),
                    AuditAction.Create,
                    null,
                    JsonSerializer.Serialize(createdProduct),
                    JsonSerializer.Serialize(new { origen = "importacion" }),
                    cancellationToken);

                created++;
                continue;
            }

            var before = await _productRepository.GetByIdAsync(tenantId, productId.Value, cancellationToken);
            if (before is null)
            {
                throw new NotFoundException("Producto no encontrado.");
            }

            var updateRequest = new ProductUpdateDto(
                string.IsNullOrWhiteSpace(nombre) ? null : nombre,
                string.IsNullOrWhiteSpace(sku) ? null : sku,
                null,
                null,
                null,
                row.Activo,
                row.PrecioBase,
                null);

            await _productRepository.UpdateAsync(tenantId, productId.Value, updateRequest, now, cancellationToken);

            if (!string.IsNullOrWhiteSpace(codigo) && !idByCode.HasValue)
            {
                await _productRepository.AddCodeAsync(tenantId, productId.Value, codigo, now, cancellationToken);
            }

            var after = await _productRepository.GetByIdAsync(tenantId, productId.Value, cancellationToken);
            if (after is null)
            {
                throw new NotFoundException("Producto no encontrado.");
            }

            await _auditLogService.LogAsync(
                "Producto",
                after.Id.ToString(),
                AuditAction.Update,
                JsonSerializer.Serialize(before),
                JsonSerializer.Serialize(after),
                JsonSerializer.Serialize(new { origen = "importacion" }),
                cancellationToken);

            updated++;
        }

        return new ProductImportConfirmResultDto(created, updated, parsed.Rows.Count);
    }

    private ParsedCsv ParseCsv(string csvContent)
    {
        if (string.IsNullOrWhiteSpace(csvContent))
        {
            return ParsedCsv.WithError("file", "El archivo CSV esta vacio.");
        }

        using var reader = new StringReader(csvContent);
        string? headerLine = null;
        var lineNumber = 0;

        while (headerLine is null && reader.Peek() >= 0)
        {
            var line = reader.ReadLine();
            lineNumber++;
            if (!string.IsNullOrWhiteSpace(line))
            {
                headerLine = line;
            }
        }

        if (headerLine is null)
        {
            return ParsedCsv.WithError("file", "El archivo CSV no tiene encabezados.");
        }

        var headers = ParseCsvLine(headerLine);
        var map = BuildHeaderMap(headers);

        if (!map.ContainsKey("sku") && !map.ContainsKey("codigo"))
        {
            return ParsedCsv.WithError("headers", "El CSV debe incluir columna sku o codigo.");
        }

        if (!map.ContainsKey("nombre"))
        {
            return ParsedCsv.WithError("headers", "El CSV debe incluir columna nombre.");
        }

        var rows = new List<ParsedRow>();
        var currentLine = lineNumber;
        string? lineContent;
        while ((lineContent = reader.ReadLine()) is not null)
        {
            currentLine++;
            if (string.IsNullOrWhiteSpace(lineContent))
            {
                continue;
            }

            var values = ParseCsvLine(lineContent);
            var sku = GetValue(values, map, "sku");
            var nombre = GetValue(values, map, "nombre");
            var codigo = GetValue(values, map, "codigo");
            var precioRaw = GetValue(values, map, "precio_base");
            var activoRaw = GetValue(values, map, "activo");

            if (string.IsNullOrWhiteSpace(sku)
                && string.IsNullOrWhiteSpace(nombre)
                && string.IsNullOrWhiteSpace(codigo)
                && string.IsNullOrWhiteSpace(precioRaw)
                && string.IsNullOrWhiteSpace(activoRaw))
            {
                continue;
            }

            var errors = new List<string>();
            decimal? precioBase = null;
            if (!string.IsNullOrWhiteSpace(precioRaw))
            {
                if (!TryParseDecimal(precioRaw, out var parsedPrecio))
                {
                    errors.Add("Precio base invalido.");
                }
                else if (parsedPrecio < 0)
                {
                    errors.Add("Precio base no puede ser negativo.");
                }
                else
                {
                    precioBase = parsedPrecio;
                }
            }

            bool? activo = null;
            if (!string.IsNullOrWhiteSpace(activoRaw))
            {
                if (!TryParseBool(activoRaw, out var parsedActivo))
                {
                    errors.Add("Activo invalido.");
                }
                else
                {
                    activo = parsedActivo;
                }
            }

            var trimmedSku = sku?.Trim();
            var trimmedNombre = nombre?.Trim();
            var trimmedCodigo = codigo?.Trim();

            if (string.IsNullOrWhiteSpace(trimmedSku) && string.IsNullOrWhiteSpace(trimmedCodigo))
            {
                errors.Add("SKU o codigo requerido.");
            }

            if (string.IsNullOrWhiteSpace(trimmedNombre))
            {
                errors.Add("Nombre requerido.");
            }

            rows.Add(new ParsedRow(currentLine, trimmedSku, trimmedNombre, trimmedCodigo, precioBase, activo, errors));
        }

        AddDuplicateErrors(rows);

        return new ParsedCsv(rows, new Dictionary<string, string[]>());
    }

    private static void AddDuplicateErrors(List<ParsedRow> rows)
    {
        var skuCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var codigoCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            if (!string.IsNullOrWhiteSpace(row.Sku))
            {
                skuCounts[row.Sku] = skuCounts.TryGetValue(row.Sku, out var count) ? count + 1 : 1;
            }

            if (!string.IsNullOrWhiteSpace(row.Codigo))
            {
                codigoCounts[row.Codigo] = codigoCounts.TryGetValue(row.Codigo, out var count) ? count + 1 : 1;
            }
        }

        foreach (var row in rows)
        {
            if (!string.IsNullOrWhiteSpace(row.Sku)
                && skuCounts.TryGetValue(row.Sku, out var skuCount)
                && skuCount > 1)
            {
                row.Errors.Add("SKU duplicado en archivo.");
            }

            if (!string.IsNullOrWhiteSpace(row.Codigo)
                && codigoCounts.TryGetValue(row.Codigo, out var codeCount)
                && codeCount > 1)
            {
                row.Errors.Add("Codigo duplicado en archivo.");
            }
        }
    }

    private static Dictionary<string, int> BuildHeaderMap(IReadOnlyList<string> headers)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < headers.Count; i++)
        {
            var normalized = NormalizeHeader(headers[i]);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                continue;
            }

            normalized = normalized switch
            {
                "codigo" or "code" => "codigo",
                "nombre" or "name" => "nombre",
                "precio" or "precio_base" or "precio-base" => "precio_base",
                "activo" or "is_active" or "habilitado" => "activo",
                _ => normalized
            };

            if (!map.ContainsKey(normalized))
            {
                map[normalized] = i;
            }
        }

        return map;
    }

    private static string NormalizeHeader(string value) =>
        value.Trim().Replace(" ", string.Empty).ToLowerInvariant();

    private static string? GetValue(IReadOnlyList<string> values, Dictionary<string, int> map, string key)
    {
        if (!map.TryGetValue(key, out var index))
        {
            return null;
        }

        if (index < 0 || index >= values.Count)
        {
            return null;
        }

        var value = values[index];
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static IReadOnlyList<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            sb.Append(ch);
        }

        result.Add(sb.ToString());
        return result;
    }

    private static bool TryParseDecimal(string input, out decimal value)
    {
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value)
            || decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
    }

    private static bool TryParseBool(string input, out bool value)
    {
        var normalized = input.Trim().ToLowerInvariant();
        if (normalized is "true" or "1" or "si" or "yes" or "y")
        {
            value = true;
            return true;
        }

        if (normalized is "false" or "0" or "no" or "n")
        {
            value = false;
            return true;
        }

        value = false;
        return false;
    }

    private Guid EnsureTenant()
    {
        if (_requestContext.TenantId == Guid.Empty)
        {
            throw new UnauthorizedException("Contexto de tenant invalido.");
        }

        return _requestContext.TenantId;
    }

    private sealed record ParsedRow(
        int RowNumber,
        string? Sku,
        string? Nombre,
        string? Codigo,
        decimal? PrecioBase,
        bool? Activo,
        List<string> Errors);

    private sealed record ParsedCsv(
        List<ParsedRow> Rows,
        Dictionary<string, string[]> Errors)
    {
        public static ParsedCsv WithError(string key, string message)
        {
            var errors = new Dictionary<string, string[]>
            {
                [key] = new[] { message }
            };
            return new ParsedCsv(new List<ParsedRow>(), errors);
        }
    }
}
