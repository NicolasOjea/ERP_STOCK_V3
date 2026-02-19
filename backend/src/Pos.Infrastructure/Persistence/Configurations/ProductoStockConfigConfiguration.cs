using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class ProductoStockConfigConfiguration : IEntityTypeConfiguration<ProductoStockConfig>
{
    public void Configure(EntityTypeBuilder<ProductoStockConfig> builder)
    {
        builder.ToTable("producto_stock_config");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.ProductoId).HasColumnType("uuid");
        builder.Property(x => x.SucursalId).HasColumnType("uuid");

        builder.Property(x => x.StockMinimo).HasColumnType("numeric(18,4)").IsRequired();
        builder.Property(x => x.StockDeseado)
            .HasColumnName("stockdeseado")
            .HasColumnType("numeric(18,4)")
            .HasDefaultValue(0m)
            .IsRequired();
        builder.Property(x => x.ToleranciaPct).HasColumnType("numeric(6,2)").HasDefaultValue(25m).IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.SucursalId);
        builder.HasIndex(x => new { x.TenantId, x.ProductoId, x.SucursalId }).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(x => x.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Sucursal>()
            .WithMany()
            .HasForeignKey(x => x.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
