using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Caja;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;
using System.Text.Json;

namespace Pos.Infrastructure.Repositories;

public sealed class CajaRepository : ICajaRepository
{
    private readonly PosDbContext _dbContext;

    public CajaRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CajaDto>> GetCajasAsync(
        Guid tenantId,
        Guid sucursalId,
        bool? activo,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Cajas.AsNoTracking()
            .Where(c => c.TenantId == tenantId && c.SucursalId == sucursalId);

        if (activo.HasValue)
        {
            query = query.Where(c => c.IsActive == activo.Value);
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CajaDto(c.Id, c.Name, c.Numero, c.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<CajaDto> CreateCajaAsync(
        Guid tenantId,
        Guid sucursalId,
        CajaCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var numero = request.Numero.Trim();
        var exists = await _dbContext.Cajas.AsNoTracking()
            .AnyAsync(c => c.TenantId == tenantId && c.SucursalId == sucursalId && c.Numero == numero, cancellationToken);
        if (exists)
        {
            throw new ConflictException("Numero de caja ya existe.");
        }

        var caja = new Caja(
            Guid.NewGuid(),
            tenantId,
            sucursalId,
            request.Nombre,
            numero,
            nowUtc,
            request.IsActive ?? true);

        _dbContext.Cajas.Add(caja);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CajaDto(caja.Id, caja.Name, caja.Numero, caja.IsActive);
    }

    public Task<bool> CajaExistsAsync(Guid tenantId, Guid sucursalId, Guid cajaId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Cajas.AsNoTracking()
            .AnyAsync(c => c.TenantId == tenantId && c.SucursalId == sucursalId && c.Id == cajaId, cancellationToken);
    }

    public Task<bool> HasOpenSessionAsync(Guid tenantId, Guid cajaId, CancellationToken cancellationToken = default)
    {
        return _dbContext.CajaSesiones.AsNoTracking()
            .AnyAsync(s => s.TenantId == tenantId && s.CajaId == cajaId && s.Estado == CajaSesionEstado.Abierta, cancellationToken);
    }

    public async Task<CajaSesionDto> OpenSessionAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid cajaId,
        decimal montoInicial,
        string turno,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var session = new CajaSesion(Guid.NewGuid(), tenantId, cajaId, sucursalId, montoInicial, turno, nowUtc, nowUtc);
        _dbContext.CajaSesiones.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CajaSesionDto(session.Id, session.CajaId, session.SucursalId, session.Turno, session.MontoInicial, session.AperturaAt, session.Estado.ToString().ToUpperInvariant());
    }

    public async Task<CajaMovimientoResultDto> AddMovimientoAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid cajaSesionId,
        CajaMovimientoTipo tipo,
        decimal montoSigned,
        string motivo,
        string medioPago,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var session = await _dbContext.CajaSesiones
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.SucursalId == sucursalId && s.Id == cajaSesionId, cancellationToken);

        if (session is null)
        {
            throw new NotFoundException("Sesion no encontrada.");
        }

        if (session.Estado != CajaSesionEstado.Abierta)
        {
            throw new ConflictException("Sesion de caja cerrada.");
        }

        var movimientosSum = await _dbContext.CajaMovimientos
            .Where(m => m.TenantId == tenantId && m.CajaSesionId == cajaSesionId)
            .SumAsync(m => (decimal?)m.Monto, cancellationToken) ?? 0m;

        var saldoAntes = session.MontoInicial + movimientosSum;
        var saldoDespues = saldoAntes + montoSigned;

        var movimiento = new CajaMovimiento(
            Guid.NewGuid(),
            tenantId,
            cajaSesionId,
            tipo,
            medioPago,
            montoSigned,
            motivo,
            nowUtc,
            nowUtc);

        _dbContext.CajaMovimientos.Add(movimiento);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var dto = new CajaMovimientoDto(
            movimiento.Id,
            movimiento.CajaSesionId,
            movimiento.Tipo.ToString().ToUpperInvariant(),
            movimiento.MedioPago,
            movimiento.Monto,
            movimiento.Motivo,
            movimiento.Fecha);

        return new CajaMovimientoResultDto(dto, saldoAntes, saldoDespues);
    }

    public async Task<CajaResumenDto?> GetResumenAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid cajaSesionId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.CajaSesiones.AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.SucursalId == sucursalId && s.Id == cajaSesionId, cancellationToken);

        if (session is null)
        {
            return null;
        }

        var movimientos = await _dbContext.CajaMovimientos.AsNoTracking()
            .Where(m => m.TenantId == tenantId && m.CajaSesionId == cajaSesionId)
            .ToListAsync(cancellationToken);

        var totalIngresos = movimientos.Where(m => m.Monto > 0).Sum(m => m.Monto);
        var totalEgresos = movimientos.Where(m => m.Monto < 0).Sum(m => -m.Monto);
        var saldoActual = session.MontoInicial + movimientos.Sum(m => m.Monto);

        var teoricoPorMedio = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var mov in movimientos)
        {
            var key = string.IsNullOrWhiteSpace(mov.MedioPago) ? "EFECTIVO" : mov.MedioPago.Trim().ToUpperInvariant();
            teoricoPorMedio[key] = (teoricoPorMedio.TryGetValue(key, out var current) ? current : 0m) + mov.Monto;
        }

        teoricoPorMedio["EFECTIVO"] = (teoricoPorMedio.TryGetValue("EFECTIVO", out var efectivo) ? efectivo : 0m) + session.MontoInicial;

        var medios = teoricoPorMedio
            .OrderBy(x => x.Key)
            .Select(x => new CajaResumenMedioDto(x.Key, x.Value))
            .ToList();

        return new CajaResumenDto(
            session.Id,
            session.CajaId,
            session.MontoInicial,
            totalIngresos,
            totalEgresos,
            saldoActual,
            movimientos.Count,
            medios);
    }

    public async Task<CajaSesionDto?> GetSesionAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid cajaSesionId,
        CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.CajaSesiones.AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.SucursalId == sucursalId && s.Id == cajaSesionId, cancellationToken);

        if (session is null)
        {
            return null;
        }

        return new CajaSesionDto(
            session.Id,
            session.CajaId,
            session.SucursalId,
            session.Turno,
            session.MontoInicial,
            session.AperturaAt,
            session.Estado.ToString().ToUpperInvariant());
    }

    public async Task<CajaCierreResultDto> CloseSessionAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid cajaSesionId,
        CajaCierreRequestDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var session = await _dbContext.CajaSesiones
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.SucursalId == sucursalId && s.Id == cajaSesionId, cancellationToken);

        if (session is null)
        {
            throw new NotFoundException("Sesion no encontrada.");
        }

        if (session.Estado == CajaSesionEstado.Cerrada)
        {
            throw new ConflictException("La sesion ya esta cerrada.");
        }

        var movimientos = await _dbContext.CajaMovimientos.AsNoTracking()
            .Where(m => m.TenantId == tenantId && m.CajaSesionId == cajaSesionId)
            .Select(m => new { m.MedioPago, m.Monto })
            .ToListAsync(cancellationToken);

        // TODO: incluir ventas por medio de pago cuando exista el modulo de ventas.
        var teoricoPorMedio = movimientos
            .GroupBy(m => string.IsNullOrWhiteSpace(m.MedioPago) ? "EFECTIVO" : m.MedioPago)
            .ToDictionary(g => g.Key.ToUpperInvariant(), g => g.Sum(x => x.Monto));

        if (teoricoPorMedio.TryGetValue("EFECTIVO", out var efectivoMov))
        {
            teoricoPorMedio["EFECTIVO"] = efectivoMov + session.MontoInicial;
        }
        else
        {
            teoricoPorMedio["EFECTIVO"] = session.MontoInicial;
        }

        var contadoPorMedio = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            ["EFECTIVO"] = request.EfectivoContado
        };

        foreach (var medio in request.Medios ?? Array.Empty<CajaCierreMedioDto>())
        {
            var key = medio.Medio.Trim().ToUpperInvariant();
            contadoPorMedio[key] = medio.Contado;
        }

        var medios = new HashSet<string>(teoricoPorMedio.Keys, StringComparer.OrdinalIgnoreCase);
        foreach (var key in contadoPorMedio.Keys)
        {
            medios.Add(key);
        }

        var detalle = medios
            .Select(medio =>
            {
                var teorico = teoricoPorMedio.TryGetValue(medio, out var t) ? t : 0m;
                var contado = contadoPorMedio.TryGetValue(medio, out var c) ? c : 0m;
                var diferencia = contado - teorico;
                return new CajaCierreMedioResultDto(medio, teorico, contado, diferencia);
            })
            .OrderBy(d => d.Medio)
            .ToList();

        var totalTeorico = detalle.Sum(d => d.Teorico);
        var totalContado = detalle.Sum(d => d.Contado);
        var diferenciaTotal = totalContado - totalTeorico;

        if (diferenciaTotal != 0m && string.IsNullOrWhiteSpace(request.MotivoDiferencia))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["motivoDiferencia"] = new[] { "El motivo es obligatorio si hay diferencia." }
                });
        }

        var arqueoJson = System.Text.Json.JsonSerializer.Serialize(detalle);

        session.Cerrar(totalContado, diferenciaTotal, request.MotivoDiferencia?.Trim(), arqueoJson, nowUtc);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new CajaCierreResultDto(
            session.Id,
            session.CajaId,
            session.SucursalId,
            session.Estado.ToString().ToUpperInvariant(),
            session.CierreAt ?? nowUtc,
            totalTeorico,
            totalContado,
            diferenciaTotal,
            detalle);
    }

    public async Task<IReadOnlyList<CajaHistorialDto>> GetSesionesHistoricasAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        CancellationToken cancellationToken = default)
    {
        var query = from s in _dbContext.CajaSesiones.AsNoTracking()
                    join c in _dbContext.Cajas.AsNoTracking() on s.CajaId equals c.Id
                    where s.TenantId == tenantId
                        && s.SucursalId == sucursalId
                        && s.Estado == CajaSesionEstado.Cerrada
                    orderby s.CierreAt descending
                    select new
                    {
                        Sesion = s,
                        CajaNombre = c.Name
                    };

        if (fromUtc.HasValue)
        {
            query = query.Where(x =>
                x.Sesion.AperturaAt >= fromUtc.Value
                && (x.Sesion.CierreAt ?? x.Sesion.UpdatedAt) >= fromUtc.Value);
        }
        if (toUtc.HasValue)
        {
            query = query.Where(x =>
                x.Sesion.AperturaAt <= toUtc.Value
                && (x.Sesion.CierreAt ?? x.Sesion.UpdatedAt) <= toUtc.Value);
        }

        var rows = await query.ToListAsync(cancellationToken);
        var result = new List<CajaHistorialDto>(rows.Count);

        foreach (var row in rows)
        {
            var medios = ParseArqueo(row.Sesion.ArqueoJson);
            decimal GetByMedio(string medio)
                => medios.FirstOrDefault(m => string.Equals(m.Medio, medio, StringComparison.OrdinalIgnoreCase))?.Teorico ?? 0m;

            var cierreAt = row.Sesion.CierreAt ?? row.Sesion.UpdatedAt;
            var numerosVentas = await _dbContext.Ventas.AsNoTracking()
                .Where(v => v.TenantId == tenantId
                            && v.SucursalId == sucursalId
                            && v.Estado == VentaEstado.Confirmada
                            && v.UpdatedAt >= row.Sesion.AperturaAt
                            && v.UpdatedAt <= cierreAt)
                .Select(v => v.Numero)
                .Where(n => n > 0)
                .ToListAsync(cancellationToken);

            result.Add(new CajaHistorialDto(
                row.Sesion.Id,
                row.Sesion.CajaId,
                row.CajaNombre,
                row.Sesion.Turno,
                row.Sesion.AperturaAt,
                row.Sesion.CierreAt,
                row.Sesion.MontoInicial,
                GetByMedio("EFECTIVO"),
                GetByMedio("TARJETA"),
                GetByMedio("TRANSFERENCIA"),
                GetByMedio("OTRO"),
                GetByMedio("APLICATIVO"),
                row.Sesion.MontoCierre ?? 0m,
                row.Sesion.DiferenciaTotal,
                row.Sesion.MotivoDiferencia,
                numerosVentas.Count > 0 ? numerosVentas.Min() : null,
                numerosVentas.Count > 0 ? numerosVentas.Max() : null));
        }

        return result;
    }

    private static IReadOnlyList<CajaCierreMedioResultDto> ParseArqueo(string? arqueoJson)
    {
        if (string.IsNullOrWhiteSpace(arqueoJson))
        {
            return Array.Empty<CajaCierreMedioResultDto>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<CajaCierreMedioResultDto>>(arqueoJson) ?? new List<CajaCierreMedioResultDto>();
        }
        catch
        {
            return Array.Empty<CajaCierreMedioResultDto>();
        }
    }
}
