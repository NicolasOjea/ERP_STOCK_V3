using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class StockMovimientoConfiguration : IEntityTypeConfiguration<StockMovimiento>
{
    public void Configure(EntityTypeBuilder<StockMovimiento> builder)
    {
        builder.ToTable("stock_movimientos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.SucursalId).HasColumnType("uuid");

        builder.Property(x => x.Tipo).HasConversion<int>().IsRequired();
        builder.Property(x => x.Motivo).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Fecha).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.VentaNumero).HasColumnType("bigint");

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.SucursalId);
        builder.HasIndex(x => x.Fecha);
        builder.HasIndex(x => new { x.TenantId, x.VentaNumero });

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Sucursal>()
            .WithMany()
            .HasForeignKey(x => x.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
