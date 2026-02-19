using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class CajaMovimientoConfiguration : IEntityTypeConfiguration<CajaMovimiento>
{
    public void Configure(EntityTypeBuilder<CajaMovimiento> builder)
    {
        builder.ToTable("caja_movimientos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.CajaSesionId).HasColumnType("uuid");

        builder.Property(x => x.Tipo).HasConversion<int>().IsRequired();
        builder.Property(x => x.MedioPago).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Monto).HasColumnType("numeric(18,4)").IsRequired();
        builder.Property(x => x.Motivo).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Fecha).HasColumnType("timestamp with time zone").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.CajaSesionId);
        builder.HasIndex(x => x.Fecha);
        builder.HasIndex(x => x.MedioPago);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CajaSesion>()
            .WithMany()
            .HasForeignKey(x => x.CajaSesionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
