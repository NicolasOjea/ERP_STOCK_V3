using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class PreRecepcionItemConfiguration : IEntityTypeConfiguration<PreRecepcionItem>
{
    public void Configure(EntityTypeBuilder<PreRecepcionItem> builder)
    {
        builder.ToTable("pre_recepcion_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.PreRecepcionId).HasColumnType("uuid");
        builder.Property(x => x.DocumentoCompraItemId).HasColumnType("uuid");
        builder.Property(x => x.Codigo).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Cantidad).HasColumnType("numeric(18,4)").IsRequired();
        builder.Property(x => x.CostoUnitario).HasColumnType("numeric(18,4)");
        builder.Property(x => x.ProductoId).HasColumnType("uuid");
        builder.Property(x => x.Estado).HasColumnType("integer").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.PreRecepcionId);
        builder.HasIndex(x => x.DocumentoCompraItemId);
        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => new { x.PreRecepcionId, x.DocumentoCompraItemId }).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PreRecepcion>()
            .WithMany()
            .HasForeignKey(x => x.PreRecepcionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DocumentoCompraItem>()
            .WithMany()
            .HasForeignKey(x => x.DocumentoCompraItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(x => x.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
