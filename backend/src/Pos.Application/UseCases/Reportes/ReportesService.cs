using Pos.Application.Abstractions;
using Pos.Application.DTOs.Reportes;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Reportes;

public sealed class ReportesService
{
    private const int DefaultTop = 10;
    private const int MaxTop = 50;

    private readonly IReportesRepository _repository;
    private readonly IRequestContext _requestContext;

    public ReportesService(
        IReportesRepository repository,
        IRequestContext requestContext)
    {
        _repository = repository;
        _requestContext = requestContext;
    }

    public async Task<ReportChartDto> GetVentasPorDiaAsync(
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        (desde, hasta) = NormalizeRangeToUtc(desde, hasta);
        ValidateRango(desde, hasta);

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var data = await _repository.GetVentasPorDiaAsync(tenantId, sucursalId, desde, hasta, cancellationToken);

        var labels = new List<string>();
        var values = new List<decimal>();

        if (data.Count > 0 && desde.HasValue && hasta.HasValue)
        {
            var startDate = desde.Value.Date;
            var endDate = hasta.Value.Date;
            var map = data.ToDictionary(d => d.Fecha.Date, d => d.Total);

            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                labels.Add(day.ToString("yyyy-MM-dd"));
                values.Add(map.TryGetValue(day.Date, out var total) ? total : 0m);
            }
        }
        else
        {
            foreach (var row in data.OrderBy(d => d.Fecha))
            {
                labels.Add(row.Fecha.ToString("yyyy-MM-dd"));
                values.Add(row.Total);
            }
        }

        var series = new List<ReportSerieDto>
        {
            new("Ventas", values)
        };

        return new ReportChartDto(labels, series);
    }

    public async Task<ReportResumenVentasDto> GetResumenVentasAsync(
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        (desde, hasta) = NormalizeRangeToUtc(desde, hasta);
        ValidateRango(desde, hasta);

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        return await _repository.GetResumenVentasAsync(tenantId, sucursalId, desde, hasta, cancellationToken);
    }

    public async Task<ReportChartDto> GetMediosPagoAsync(
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        (desde, hasta) = NormalizeRangeToUtc(desde, hasta);
        ValidateRango(desde, hasta);

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var data = await _repository.GetMediosPagoAsync(tenantId, sucursalId, desde, hasta, cancellationToken);
        var labels = data.Select(d => d.MedioPago).ToList();
        var values = data.Select(d => d.Total).ToList();

        var series = new List<ReportSerieDto>
        {
            new("Total", values)
        };

        return new ReportChartDto(labels, series);
    }

    public async Task<ReportTableDto<TopProductoItemDto>> GetTopProductosAsync(
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        int? top,
        CancellationToken cancellationToken)
    {
        (desde, hasta) = NormalizeRangeToUtc(desde, hasta);
        ValidateRango(desde, hasta);

        var finalTop = top ?? DefaultTop;
        if (finalTop <= 0 || finalTop > MaxTop)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["top"] = new[] { $"Top debe estar entre 1 y {MaxTop}." }
                });
        }

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var data = await _repository.GetTopProductosAsync(tenantId, sucursalId, desde, hasta, finalTop, cancellationToken);
        return new ReportTableDto<TopProductoItemDto>(data);
    }

    public async Task<ReportTableDto<RotacionStockItemDto>> GetRotacionStockAsync(
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        (desde, hasta) = NormalizeRangeToUtc(desde, hasta);
        ValidateRango(desde, hasta);

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var data = await _repository.GetRotacionStockAsync(tenantId, sucursalId, desde, hasta, cancellationToken);
        return new ReportTableDto<RotacionStockItemDto>(data);
    }

    public async Task<ReportTableDto<StockInmovilizadoItemDto>> GetStockInmovilizadoAsync(
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        (desde, hasta) = NormalizeRangeToUtc(desde, hasta);
        ValidateRango(desde, hasta);

        var tenantId = EnsureTenant();
        var sucursalId = EnsureSucursal();

        var data = await _repository.GetStockInmovilizadoAsync(tenantId, sucursalId, desde, hasta, cancellationToken);
        return new ReportTableDto<StockInmovilizadoItemDto>(data);
    }

    private static void ValidateRango(DateTimeOffset? desde, DateTimeOffset? hasta)
    {
        if (desde.HasValue && hasta.HasValue && desde > hasta)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["fecha"] = new[] { "El rango de fechas es invalido." }
                });
        }
    }

    private static (DateTimeOffset? Desde, DateTimeOffset? Hasta) NormalizeRangeToUtc(
        DateTimeOffset? desde,
        DateTimeOffset? hasta)
    {
        var desdeUtc = desde?.ToUniversalTime();
        var hastaUtc = hasta?.ToUniversalTime();
        return (desdeUtc, hastaUtc);
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
