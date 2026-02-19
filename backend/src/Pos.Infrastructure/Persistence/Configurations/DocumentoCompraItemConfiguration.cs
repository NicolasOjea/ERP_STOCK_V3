using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class DocumentoCompraItemConfiguration : IEntityTypeConfiguration<DocumentoCompraItem>
{
    public void Configure(EntityTypeBuilder<DocumentoCompraItem> builder)
    {
        builder.ToTable("documento_compra_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.DocumentoCompraId).HasColumnType("uuid");
        builder.Property(x => x.Codigo).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Cantidad).HasColumnType("numeric(18,4)").IsRequired();
        builder.Property(x => x.CostoUnitario).HasColumnType("numeric(18,4)");

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.DocumentoCompraId);
        builder.HasIndex(x => x.Codigo);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DocumentoCompra>()
            .WithMany()
            .HasForeignKey(x => x.DocumentoCompraId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
