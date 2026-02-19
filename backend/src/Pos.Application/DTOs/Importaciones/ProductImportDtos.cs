namespace Pos.Application.DTOs.Importaciones;

public sealed record ProductImportRowDto(
    int RowNumber,
    string? Sku,
    string? Nombre,
    string? Codigo,
    decimal? PrecioBase,
    bool? Activo,
    IReadOnlyList<string> Errores,
    bool IsValid);

public sealed record ProductImportPreviewDto(
    IReadOnlyList<ProductImportRowDto> Rows,
    int ValidCount,
    int ErrorCount);

public sealed record ProductImportConfirmResultDto(
    int Created,
    int Updated,
    int Total);
