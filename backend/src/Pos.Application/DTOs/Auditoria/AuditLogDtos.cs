namespace Pos.Application.DTOs.Auditoria;

public sealed record AuditLogQueryRequestDto(
    string? Entidad,
    Guid? UsuarioId,
    DateTimeOffset? Desde,
    DateTimeOffset? Hasta,
    int Page,
    int Size);

public sealed record AuditLogListItemDto(
    Guid Id,
    string Entidad,
    string EntidadId,
    string Accion,
    Guid? UsuarioId,
    DateTimeOffset Fecha,
    string? Metadata);

public sealed record AuditLogQueryResultDto(
    IReadOnlyList<AuditLogListItemDto> Items,
    int Page,
    int Size,
    int Total);
