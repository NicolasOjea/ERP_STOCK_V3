using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.DocumentosCompra;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class DocumentoCompraTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public DocumentoCompraTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Parse_crea_documento()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.ComprasRegistrar);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            proveedorId = (string?)null,
            numero = $"REM-{Guid.NewGuid():N}",
            fecha = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            items = new[]
            {
                new { codigo = "ABC-123", descripcion = "Item", cantidad = 2m, costoUnitario = 10.5m }
            }
        };

        var response = await client.PostAsJsonAsync("/api/v1/documentos-compra/parse", payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<DocumentoCompraParseResultDto>();
        Assert.NotNull(result);
        Assert.Single(result!.Items);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var documento = await db.DocumentosCompra.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == result.DocumentoCompraId);
        Assert.NotNull(documento);

        var items = await db.DocumentoCompraItems.AsNoTracking()
            .Where(i => i.DocumentoCompraId == result.DocumentoCompraId)
            .ToListAsync();
        Assert.Single(items);
    }
}
