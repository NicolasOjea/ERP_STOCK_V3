namespace Pos.Application.DTOs.Etiquetas;

public sealed record EtiquetaRequestDto(IReadOnlyCollection<Guid> ProductoIds);

public sealed record EtiquetaItemDto(
    Guid ProductoId,
    string Nombre,
    decimal Precio,
    string CodigoBarra);

public sealed record EtiquetaPdfDataDto(IReadOnlyList<EtiquetaItemDto> Items);
