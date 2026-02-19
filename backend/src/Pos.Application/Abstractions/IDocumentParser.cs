using System.Text.Json;
using Pos.Application.DTOs.DocumentosCompra;

namespace Pos.Application.Abstractions;

public interface IDocumentParser
{
    ParsedDocumentDto Parse(JsonElement input);
}
