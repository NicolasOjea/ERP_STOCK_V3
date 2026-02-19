using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class ComprobanteConfiguration : IEntityTypeConfiguration<Comprobante>
{
    public void Configure(EntityTypeBuilder<Comprobante> builder)
    {
        builder.ToTable("comprobantes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.SucursalId).HasColumnType("uuid");
        builder.Property(x => x.VentaId).HasColumnType("uuid");
        builder.Property(x => x.UserId).HasColumnType("uuid");

        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();
        builder.Property(x => x.Total).HasColumnType("numeric(18,4)").HasDefaultValue(0m);
        builder.Property(x => x.Numero).HasMaxLength(120);
        builder.Property(x => x.FiscalProvider).HasMaxLength(50);
        builder.Property(x => x.FiscalPayload).HasColumnType("text");
        builder.Property(x => x.EmitidoAt).HasColumnType("timestamp with time zone");

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.SucursalId);
        builder.HasIndex(x => x.VentaId);
        builder.HasIndex(x => x.Estado);
        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => new { x.TenantId, x.VentaId })
            .IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Sucursal>()
            .WithMany()
            .HasForeignKey(x => x.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Venta>()
            .WithMany()
            .HasForeignKey(x => x.VentaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
