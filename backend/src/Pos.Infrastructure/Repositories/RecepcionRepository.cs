using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Recepciones;
using Pos.Application.DTOs.Stock;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class RecepcionRepository : IRecepcionRepository
{
    private readonly PosDbContext _dbContext;

    public RecepcionRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RecepcionConfirmResultDto> ConfirmarAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid preRecepcionId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var preRecepcion = await _dbContext.PreRecepciones
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.SucursalId == sucursalId && p.Id == preRecepcionId, cancellationToken);

        if (preRecepcion is null)
        {
            throw new NotFoundException("Pre-recepcion no encontrada.");
        }

        var alreadyConfirmed = await _dbContext.Recepciones.AsNoTracking()
            .AnyAsync(r => r.TenantId == tenantId && r.PreRecepcionId == preRecepcionId, cancellationToken);

        if (alreadyConfirmed)
        {
            throw new ConflictException("La pre-recepcion ya fue confirmada.");
        }

        var preItems = await _dbContext.PreRecepcionItems
            .Where(i => i.TenantId == tenantId && i.PreRecepcionId == preRecepcionId)
            .ToListAsync(cancellationToken);

        if (preItems.Count == 0)
        {
            throw new ConflictException("La pre-recepcion no tiene items.");
        }

        if (preItems.Any(i => i.ProductoId is null || i.Estado != PreRecepcionItemEstado.Ok))
        {
            throw new ConflictException("Hay items sin match confirmado.");
        }

        var recepcion = new Recepcion(Guid.NewGuid(), tenantId, sucursalId, preRecepcionId, nowUtc);
        _dbContext.Recepciones.Add(recepcion);

        var recepcionItems = preItems.Select(item => new RecepcionItem(
            Guid.NewGuid(),
            tenantId,
            recepcion.Id,
            item.Id,
            item.ProductoId!.Value,
            item.Codigo,
            item.Descripcion,
            item.Cantidad,
            item.CostoUnitario,
            nowUtc)).ToList();

        _dbContext.RecepcionItems.AddRange(recepcionItems);

        var productIds = recepcionItems.Select(i => i.ProductoId).Distinct().ToList();
        var products = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && productIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.Sku })
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        var saldos = await _dbContext.StockSaldos
            .Where(s => s.TenantId == tenantId && s.SucursalId == sucursalId && productIds.Contains(s.ProductoId))
            .ToListAsync(cancellationToken);

        var saldoByProduct = saldos.ToDictionary(s => s.ProductoId, s => s);
        var addedSaldo = false;
        foreach (var productId in productIds)
        {
            if (saldoByProduct.ContainsKey(productId))
            {
                continue;
            }

            var saldo = new StockSaldo(Guid.NewGuid(), tenantId, productId, sucursalId, 0m, nowUtc);
            _dbContext.StockSaldos.Add(saldo);
            saldoByProduct[productId] = saldo;
            addedSaldo = true;
        }

        if (addedSaldo)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var movimientoId = Guid.NewGuid();
        var movimiento = new StockMovimiento(
            movimientoId,
            tenantId,
            sucursalId,
            StockMovimientoTipo.EntradaCompra,
            $"Recepcion {recepcion.Id}",
            nowUtc,
            nowUtc);

        _dbContext.StockMovimientos.Add(movimiento);

        var movimientoItems = new List<StockMovimientoItem>();
        var cambios = new List<StockSaldoChangeDto>();
        foreach (var item in recepcionItems)
        {
            var saldo = saldoByProduct[item.ProductoId];
            var before = saldo.CantidadActual;
            var after = before + item.Cantidad;

            saldo.SetCantidad(after, nowUtc);

            var itemId = Guid.NewGuid();
            movimientoItems.Add(new StockMovimientoItem(
                itemId,
                tenantId,
                movimientoId,
                item.ProductoId,
                item.Cantidad,
                true,
                nowUtc));

            cambios.Add(new StockSaldoChangeDto(
                movimientoId,
                itemId,
                item.ProductoId,
                before,
                after));
        }

        _dbContext.StockMovimientoItems.AddRange(movimientoItems);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var itemsDto = recepcionItems.Select(i =>
        {
            var product = products.Single(p => p.Id == i.ProductoId);
            return new RecepcionItemDto(
                i.Id,
                i.ProductoId,
                product.Name,
                product.Sku,
                i.Codigo,
                i.Descripcion,
                i.Cantidad,
                i.CostoUnitario);
        }).OrderBy(i => i.Producto).ToList();

        var recepcionDto = new RecepcionDto(
            recepcion.Id,
            recepcion.PreRecepcionId,
            recepcion.CreatedAt,
            itemsDto);

        return new RecepcionConfirmResultDto(recepcionDto, cambios);
    }
}
