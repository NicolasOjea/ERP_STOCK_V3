namespace Pos.Application.DTOs.Ventas;

public sealed record VentaDto(
    Guid Id,
    long Numero,
    Guid SucursalId,
    Guid? UserId,
    string Estado,
    string ListaPrecio,
    decimal TotalNeto,
    decimal TotalPagos,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<VentaItemDto> Items,
    bool Facturada = false);

public sealed record VentaTicketDto(
    VentaDto Venta,
    IReadOnlyCollection<VentaPagoDto> Pagos);
