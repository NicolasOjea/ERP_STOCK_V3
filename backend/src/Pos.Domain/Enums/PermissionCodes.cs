namespace Pos.Domain.Enums;

public static class PermissionCodes
{
    public const string VentaCrear = "VENTA_CREAR";
    public const string VentaConfirmar = "VENTA_CONFIRMAR";
    public const string VentaAnular = "VENTA_ANULAR";
    public const string DevolucionRegistrar = "DEVOLUCION_REGISTRAR";
    public const string CajaAbrir = "CAJA_ABRIR";
    public const string CajaCerrar = "CAJA_CERRAR";
    public const string CajaMovimiento = "CAJA_MOVIMIENTO";
    public const string StockAjustar = "STOCK_AJUSTAR";
    public const string ProductoVer = "PRODUCTO_VER";
    public const string ProductoEditar = "PRODUCTO_EDITAR";
    public const string UsuarioAdmin = "USUARIO_ADMIN";
    public const string ReportesVer = "REPORTES_VER";
    public const string ClienteGestionar = "CLIENTE_GESTIONAR";
    public const string ProveedorGestionar = "PROVEEDOR_GESTIONAR";
    public const string ComprasRegistrar = "COMPRAS_REGISTRAR";
    public const string ConfiguracionVer = "CONFIGURACION_VER";

    public static readonly string[] All =
    [
        VentaCrear,
        VentaConfirmar,
        VentaAnular,
        DevolucionRegistrar,
        CajaAbrir,
        CajaCerrar,
        CajaMovimiento,
        StockAjustar,
        ProductoVer,
        ProductoEditar,
        UsuarioAdmin,
        ReportesVer,
        ClienteGestionar,
        ProveedorGestionar,
        ComprasRegistrar,
        ConfiguracionVer
    ];
}
