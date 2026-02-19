using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("ventas");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.SucursalId).HasColumnType("uuid");
        builder.Property(x => x.UserId).HasColumnType("uuid");

        builder.Property(x => x.ListaPrecio).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();
        builder.Property(x => x.Numero)
            .HasColumnName("numero")
            .HasColumnType("bigint")
            .HasDefaultValueSql("nextval('venta_numero_seq')")
            .ValueGeneratedOnAdd();
        builder.Property(x => x.TotalNeto).HasColumnType("numeric(18,4)").HasDefaultValue(0m);
        builder.Property(x => x.TotalPagos).HasColumnType("numeric(18,4)").HasDefaultValue(0m);
        builder.Property(x => x.Facturada).HasColumnName("facturada").HasDefaultValue(false);

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.SucursalId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Estado);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => new { x.TenantId, x.Numero }).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Sucursal>()
            .WithMany()
            .HasForeignKey(x => x.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
