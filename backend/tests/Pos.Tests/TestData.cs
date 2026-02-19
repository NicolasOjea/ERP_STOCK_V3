using System.Net.Http.Json;
using Pos.Application.DTOs.Proveedores;
using Pos.Application.DTOs.Caja;

namespace Pos.Tests;

public static class TestData
{
    public static async Task<Guid> CreateProveedorAsync(HttpClient client, string? name = null)
    {
        var payload = new ProveedorCreateDto(
            name ?? $"Proveedor {Guid.NewGuid():N}",
            "1133445566",
            null,
            null,
            true);

        var response = await client.PostAsJsonAsync("/api/v1/proveedores", payload);
        response.EnsureSuccessStatusCode();

        var proveedor = await response.Content.ReadFromJsonAsync<ProveedorDto>();
        if (proveedor is null)
        {
            throw new InvalidOperationException("No se pudo crear proveedor de prueba.");
        }

        return proveedor.Id;
    }

    public static async Task<CajaSesionDto> OpenCajaSessionAsync(HttpClient client)
    {
        var numero = Random.Shared.Next(1000, 999999).ToString();
        var createResponse = await client.PostAsJsonAsync("/api/v1/caja", new CajaCreateDto(
            numero,
            $"Caja {numero}",
            true));
        createResponse.EnsureSuccessStatusCode();

        var caja = await createResponse.Content.ReadFromJsonAsync<CajaDto>();
        if (caja is null)
        {
            throw new InvalidOperationException("No se pudo crear caja de prueba.");
        }

        var abrirResponse = await client.PostAsJsonAsync("/api/v1/caja/sesiones/abrir", new CajaSesionAbrirDto(caja.Id, 0m));
        abrirResponse.EnsureSuccessStatusCode();

        var sesion = await abrirResponse.Content.ReadFromJsonAsync<CajaSesionDto>();
        if (sesion is null)
        {
            throw new InvalidOperationException("No se pudo abrir sesion de caja.");
        }

        return sesion;
    }
}
