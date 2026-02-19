using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Empresa;
using Pos.Domain.Entities;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class EmpresaDatosRepository : IEmpresaDatosRepository
{
    private readonly PosDbContext _dbContext;

    public EmpresaDatosRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EmpresaDatosDto?> GetAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmpresaDatos.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .Select(x => new EmpresaDatosDto(
                x.Id,
                x.RazonSocial,
                x.Cuit,
                x.Telefono,
                x.Direccion,
                x.Email,
                x.Web,
                x.Observaciones))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<EmpresaDatosDto> UpsertAsync(
        Guid tenantId,
        EmpresaDatosUpsertDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.EmpresaDatos
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

        if (entity is null)
        {
            entity = new EmpresaDatos(
                Guid.NewGuid(),
                tenantId,
                request.RazonSocial,
                request.Cuit,
                request.Telefono,
                request.Direccion,
                request.Email,
                request.Web,
                request.Observaciones,
                nowUtc);

            _dbContext.EmpresaDatos.Add(entity);
        }
        else
        {
            entity.Update(
                request.RazonSocial,
                request.Cuit,
                request.Telefono,
                request.Direccion,
                request.Email,
                request.Web,
                request.Observaciones,
                nowUtc);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new EmpresaDatosDto(
            entity.Id,
            entity.RazonSocial,
            entity.Cuit,
            entity.Telefono,
            entity.Direccion,
            entity.Email,
            entity.Web,
            entity.Observaciones);
    }
}
