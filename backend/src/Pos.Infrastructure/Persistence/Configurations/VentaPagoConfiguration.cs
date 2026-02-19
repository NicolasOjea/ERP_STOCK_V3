using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class VentaPagoConfiguration : IEntityTypeConfiguration<VentaPago>
{
    public void Configure(EntityTypeBuilder<VentaPago> builder)
    {
        builder.ToTable("venta_pagos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.VentaId).HasColumnType("uuid");

        builder.Property(x => x.MedioPago).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Monto).HasColumnType("numeric(18,4)").IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.VentaId);
        builder.HasIndex(x => x.MedioPago);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Venta>()
            .WithMany()
            .HasForeignKey(x => x.VentaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
