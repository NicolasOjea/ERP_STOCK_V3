using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class StockMovimientoItemConfiguration : IEntityTypeConfiguration<StockMovimientoItem>
{
    public void Configure(EntityTypeBuilder<StockMovimientoItem> builder)
    {
        builder.ToTable("stock_movimiento_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.MovimientoId).HasColumnType("uuid");
        builder.Property(x => x.ProductoId).HasColumnType("uuid");

        builder.Property(x => x.Cantidad).HasColumnType("numeric(18,4)").IsRequired();
        builder.Property(x => x.EsIngreso).IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.MovimientoId);
        builder.HasIndex(x => x.ProductoId);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<StockMovimiento>()
            .WithMany()
            .HasForeignKey(x => x.MovimientoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(x => x.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
