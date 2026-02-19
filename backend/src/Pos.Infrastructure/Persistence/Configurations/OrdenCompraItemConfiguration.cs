using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class OrdenCompraItemConfiguration : IEntityTypeConfiguration<OrdenCompraItem>
{
    public void Configure(EntityTypeBuilder<OrdenCompraItem> builder)
    {
        builder.ToTable("orden_compra_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.OrdenCompraId).HasColumnType("uuid");
        builder.Property(x => x.ProductoId).HasColumnType("uuid");
        builder.Property(x => x.Cantidad).HasColumnType("numeric(18,4)").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.OrdenCompraId);
        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => new { x.OrdenCompraId, x.ProductoId }).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrdenCompra>()
            .WithMany()
            .HasForeignKey(x => x.OrdenCompraId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(x => x.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
