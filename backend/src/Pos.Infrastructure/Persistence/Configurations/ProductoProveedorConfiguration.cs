using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class ProductoProveedorConfiguration : IEntityTypeConfiguration<ProductoProveedor>
{
    public void Configure(EntityTypeBuilder<ProductoProveedor> builder)
    {
        builder.ToTable("producto_proveedor");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.ProductoId).HasColumnType("uuid");
        builder.Property(x => x.ProveedorId).HasColumnType("uuid");
        builder.Property(x => x.EsPrincipal).HasColumnType("boolean").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => x.ProveedorId);
        builder.HasIndex(x => new { x.TenantId, x.ProductoId, x.ProveedorId }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.ProductoId })
            .IsUnique()
            .HasFilter("\"EsPrincipal\" = true");

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(x => x.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Proveedor>()
            .WithMany()
            .HasForeignKey(x => x.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
