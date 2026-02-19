using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.DocumentosCompra;
using Pos.Domain.Exceptions;

namespace Pos.Infrastructure.Adapters;

public sealed class JsonDocumentParser : IDocumentParser
{
    public ParsedDocumentDto Parse(JsonElement input)
    {
        if (input.ValueKind != JsonValueKind.Object)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["input"] = new[] { "El documento debe ser un objeto JSON." }
                });
        }

        Guid? proveedorId = null;
        if (input.TryGetProperty("proveedorId", out var proveedorProp) && proveedorProp.ValueKind != JsonValueKind.Null)
        {
            if (proveedorProp.ValueKind != JsonValueKind.String || !Guid.TryParse(proveedorProp.GetString(), out var proveedorGuid))
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["proveedorId"] = new[] { "El proveedorId es invalido." }
                    });
            }

            proveedorId = proveedorGuid;
        }

        if (!input.TryGetProperty("numero", out var numeroProp) || numeroProp.ValueKind != JsonValueKind.String)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["numero"] = new[] { "El numero es obligatorio." }
                });
        }

        var numero = numeroProp.GetString() ?? string.Empty;

        if (!input.TryGetProperty("fecha", out var fechaProp))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["fecha"] = new[] { "La fecha es obligatoria." }
                });
        }

        DateTime fecha;
        if (fechaProp.ValueKind == JsonValueKind.String && DateTime.TryParse(fechaProp.GetString(), out var parsedDate))
        {
            fecha = parsedDate.Date;
        }
        else
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["fecha"] = new[] { "La fecha es invalida." }
                });
        }

        if (!input.TryGetProperty("items", out var itemsProp) || itemsProp.ValueKind != JsonValueKind.Array)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["items"] = new[] { "Los items son obligatorios." }
                });
        }

        var items = new List<ParsedDocumentItemDto>();
        foreach (var item in itemsProp.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            var sku = item.TryGetProperty("sku", out var skuProp) && skuProp.ValueKind == JsonValueKind.String
                ? skuProp.GetString() ?? string.Empty
                : string.Empty;

            var codigo = !string.IsNullOrWhiteSpace(sku)
                ? sku
                : item.TryGetProperty("codigo", out var codigoProp) && codigoProp.ValueKind == JsonValueKind.String
                    ? codigoProp.GetString() ?? string.Empty
                    : string.Empty;

            var descripcion = item.TryGetProperty("descripcion", out var descProp) && descProp.ValueKind == JsonValueKind.String
                ? descProp.GetString() ?? string.Empty
                : string.Empty;

            var cantidad = item.TryGetProperty("cantidad", out var qtyProp) && qtyProp.ValueKind == JsonValueKind.Number
                ? qtyProp.GetDecimal()
                : 0m;

            decimal? costoUnitario = null;
            if (item.TryGetProperty("costoUnitario", out var costoProp))
            {
                if (costoProp.ValueKind == JsonValueKind.Number)
                {
                    costoUnitario = costoProp.GetDecimal();
                }
            }

            items.Add(new ParsedDocumentItemDto(codigo, descripcion, cantidad, costoUnitario));
        }

        return new ParsedDocumentDto(proveedorId, numero, fecha, items);
    }
}
