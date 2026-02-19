using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class ListaPrecioItemConfiguration : IEntityTypeConfiguration<ListaPrecioItem>
{
    public void Configure(EntityTypeBuilder<ListaPrecioItem> builder)
    {
        builder.ToTable("lista_precio_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.ListaPrecioId).HasColumnType("uuid");
        builder.Property(x => x.ProductoId).HasColumnType("uuid");
        builder.Property(x => x.Precio).HasColumnType("numeric(18,4)").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.ListaPrecioId);
        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => new { x.TenantId, x.ListaPrecioId, x.ProductoId }).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ListaPrecio>()
            .WithMany()
            .HasForeignKey(x => x.ListaPrecioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(x => x.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
