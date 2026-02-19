using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class EmpresaDatos : EntityBase
{
    private EmpresaDatos()
    {
    }

    public EmpresaDatos(
        Guid id,
        Guid tenantId,
        string razonSocial,
        string? cuit,
        string? telefono,
        string? direccion,
        string? email,
        string? web,
        string? observaciones,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(razonSocial)) throw new ArgumentException("RazonSocial is required.", nameof(razonSocial));

        RazonSocial = razonSocial;
        Cuit = cuit;
        Telefono = telefono;
        Direccion = direccion;
        Email = email;
        Web = web;
        Observaciones = observaciones;
    }

    public string RazonSocial { get; private set; } = string.Empty;
    public string? Cuit { get; private set; }
    public string? Telefono { get; private set; }
    public string? Direccion { get; private set; }
    public string? Email { get; private set; }
    public string? Web { get; private set; }
    public string? Observaciones { get; private set; }

    public void Update(
        string razonSocial,
        string? cuit,
        string? telefono,
        string? direccion,
        string? email,
        string? web,
        string? observaciones,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(razonSocial)) throw new ArgumentException("RazonSocial is required.", nameof(razonSocial));

        RazonSocial = razonSocial;
        Cuit = cuit;
        Telefono = telefono;
        Direccion = direccion;
        Email = email;
        Web = web;
        Observaciones = observaciones;
        MarkUpdated(updatedAtUtc);
    }
}
