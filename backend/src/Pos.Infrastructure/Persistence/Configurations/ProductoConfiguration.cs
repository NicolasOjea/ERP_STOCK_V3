using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("productos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.CategoriaId).HasColumnType("uuid");
        builder.Property(x => x.MarcaId).HasColumnType("uuid");
        builder.Property(x => x.ProveedorId).HasColumnType("uuid");

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Sku).HasMaxLength(80).IsRequired();
        builder.Property(x => x.PrecioBase).HasColumnType("numeric(18,4)").HasDefaultValue(1m).IsRequired();
        builder.Property(x => x.PrecioVenta).HasColumnType("numeric(18,4)").HasDefaultValue(1m).IsRequired();
        builder.Property(x => x.PricingMode).HasConversion<int>().HasDefaultValue(Pos.Domain.Enums.ProductPricingMode.FijoPct).IsRequired();
        builder.Property(x => x.MargenGananciaPct).HasColumnType("numeric(6,2)");
        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => new { x.TenantId, x.Sku }).IsUnique();
        builder.HasIndex(x => x.CategoriaId);
        builder.HasIndex(x => x.MarcaId);
        builder.HasIndex(x => x.ProveedorId);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Categoria>()
            .WithMany()
            .HasForeignKey(x => x.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Marca>()
            .WithMany()
            .HasForeignKey(x => x.MarcaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Proveedor>()
            .WithMany()
            .HasForeignKey(x => x.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
