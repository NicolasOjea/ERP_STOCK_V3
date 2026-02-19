using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class EmpresaDatosConfiguration : IEntityTypeConfiguration<EmpresaDatos>
{
    public void Configure(EntityTypeBuilder<EmpresaDatos> builder)
    {
        builder.ToTable("empresa_datos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");

        builder.Property(x => x.RazonSocial).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Cuit).HasMaxLength(32);
        builder.Property(x => x.Telefono).HasMaxLength(64);
        builder.Property(x => x.Direccion).HasMaxLength(300);
        builder.Property(x => x.Email).HasMaxLength(160);
        builder.Property(x => x.Web).HasMaxLength(180);
        builder.Property(x => x.Observaciones).HasMaxLength(1000);

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
