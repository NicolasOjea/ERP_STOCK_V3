using Microsoft.Extensions.DependencyInjection;
using Pos.Application.Abstractions;
using Pos.Application.UseCases.Auth;
using Pos.Application.UseCases.Health;
using Pos.Application.UseCases.Users;
using Pos.Application.UseCases.Products;
using Pos.Application.UseCases.Stock;
using Pos.Application.UseCases.Caja;
using Pos.Application.UseCases.Ventas;
using Pos.Application.UseCases.Proveedores;
using Pos.Application.UseCases.Compras;
using Pos.Application.UseCases.DocumentosCompra;
using Pos.Application.UseCases.PreRecepciones;
using Pos.Application.UseCases.ListasPrecio;
using Pos.Application.UseCases.Pricing;
using Pos.Application.UseCases.Devoluciones;
using Pos.Application.UseCases.Importaciones;
using Pos.Application.UseCases.Etiquetas;
using Pos.Application.UseCases.Auditoria;
using Pos.Application.UseCases.Reportes;
using Pos.Application.UseCases.Comprobantes;
using Pos.Application.UseCases.Pricing.Strategies;
using Pos.Application.UseCases.Categorias;
using Pos.Application.UseCases.Empresa;

namespace Pos.Application.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<LoginService>();
        services.AddScoped<UpdateUserRolesService>();
        services.AddScoped<ProductService>();
        services.AddScoped<StockService>();
        services.AddScoped<CajaService>();
        services.AddScoped<VentaService>();
        services.AddScoped<ProveedorService>();
        services.AddScoped<OrdenCompraService>();
        services.AddScoped<DocumentoCompraService>();
        services.AddScoped<PreRecepcionService>();
        services.AddScoped<ListaPrecioService>();
        services.AddScoped<DevolucionService>();
        services.AddScoped<ImportacionProductosService>();
        services.AddScoped<EtiquetasService>();
        services.AddScoped<AuditoriaService>();
        services.AddScoped<ReportesService>();
        services.AddScoped<ComprobantesService>();
        services.AddScoped<CategoriaPrecioService>();
        services.AddScoped<EmpresaDatosService>();
        services.AddScoped<IPromoStrategy, PorcCategoriaPromoStrategy>();
        services.AddScoped<IPromoStrategy, DosPorUnoPromoStrategy>();
        services.AddScoped<IPromoStrategy, ComboPromoStrategy>();
        services.AddScoped<PricingService>();
        return services;
    }
}
