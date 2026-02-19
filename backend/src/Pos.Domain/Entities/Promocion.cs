using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class Promocion : EntityBase
{
    private Promocion()
    {
    }

    public Promocion(
        Guid id,
        Guid tenantId,
        string nombre,
        PromocionTipo tipo,
        decimal porcentaje,
        int prioridad,
        DateTimeOffset vigenciaDesde,
        DateTimeOffset? vigenciaHasta,
        DateTimeOffset createdAtUtc,
        bool isActive = true)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre is required.", nameof(nombre));
        if (porcentaje <= 0 || porcentaje > 100) throw new ArgumentException("Porcentaje must be > 0 and <= 100.", nameof(porcentaje));

        Nombre = nombre;
        Tipo = tipo;
        Porcentaje = porcentaje;
        Prioridad = prioridad;
        VigenciaDesde = vigenciaDesde;
        VigenciaHasta = vigenciaHasta;
        IsActive = isActive;
    }

    public string Nombre { get; private set; } = string.Empty;
    public PromocionTipo Tipo { get; private set; }
    public decimal Porcentaje { get; private set; }
    public int Prioridad { get; private set; }
    public DateTimeOffset VigenciaDesde { get; private set; }
    public DateTimeOffset? VigenciaHasta { get; private set; }
    public bool IsActive { get; private set; }
}
