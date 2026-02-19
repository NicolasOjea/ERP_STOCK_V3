using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class NotaCreditoInternaConfiguration : IEntityTypeConfiguration<NotaCreditoInterna>
{
    public void Configure(EntityTypeBuilder<NotaCreditoInterna> builder)
    {
        builder.ToTable("nota_credito_interna");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.SucursalId).HasColumnType("uuid");
        builder.Property(x => x.DevolucionId).HasColumnType("uuid");

        builder.Property(x => x.Total).HasColumnType("numeric(18,4)").HasDefaultValue(0m);
        builder.Property(x => x.Motivo).HasMaxLength(500).IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.SucursalId);
        builder.HasIndex(x => x.DevolucionId).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Sucursal>()
            .WithMany()
            .HasForeignKey(x => x.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Devolucion>()
            .WithMany()
            .HasForeignKey(x => x.DevolucionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
