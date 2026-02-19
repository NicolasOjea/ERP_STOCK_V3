namespace Pos.Application.DTOs;

public sealed record HealthStatusDto(string Status, DateTimeOffset TimestampUtc);
