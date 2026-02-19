using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class DevolucionItemConfiguration : IEntityTypeConfiguration<DevolucionItem>
{
    public void Configure(EntityTypeBuilder<DevolucionItem> builder)
    {
        builder.ToTable("devolucion_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.DevolucionId).HasColumnType("uuid");
        builder.Property(x => x.ProductoId).HasColumnType("uuid");

        builder.Property(x => x.Cantidad).HasColumnType("numeric(18,4)");
        builder.Property(x => x.PrecioUnitario).HasColumnType("numeric(18,4)");

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.DevolucionId);
        builder.HasIndex(x => x.ProductoId);
        builder.HasIndex(x => x.TenantId);

        builder.HasIndex(x => new { x.DevolucionId, x.ProductoId })
            .IsUnique();

        builder.HasOne<Devolucion>()
            .WithMany()
            .HasForeignKey(x => x.DevolucionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(x => x.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
