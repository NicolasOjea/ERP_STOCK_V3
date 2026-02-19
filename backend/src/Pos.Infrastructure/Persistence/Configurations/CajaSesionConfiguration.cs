using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;
using Pos.Domain.Enums;

namespace Pos.Infrastructure.Persistence.Configurations;

public sealed class CajaSesionConfiguration : IEntityTypeConfiguration<CajaSesion>
{
    public void Configure(EntityTypeBuilder<CajaSesion> builder)
    {
        builder.ToTable("caja_sesiones");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnType("uuid");
        builder.Property(x => x.TenantId).HasColumnType("uuid");
        builder.Property(x => x.CajaId).HasColumnType("uuid");
        builder.Property(x => x.SucursalId).HasColumnType("uuid");
        builder.Property(x => x.Turno).HasColumnName("turno").HasMaxLength(16).IsRequired();

        builder.Property(x => x.MontoInicial).HasColumnType("numeric(18,4)").IsRequired();
        builder.Property(x => x.MontoCierre).HasColumnType("numeric(18,4)");
        builder.Property(x => x.DiferenciaTotal).HasColumnType("numeric(18,4)").HasDefaultValue(0m);
        builder.Property(x => x.MotivoDiferencia).HasMaxLength(500);
        builder.Property(x => x.ArqueoJson).HasColumnType("jsonb");
        builder.Property(x => x.AperturaAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.CierreAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.CajaId);
        builder.HasIndex(x => x.SucursalId);
        builder.HasIndex(x => x.AperturaAt);
        builder.HasIndex(x => new { x.CajaId, x.Estado })
            .IsUnique()
            .HasFilter($"\"Estado\" = {(int)CajaSesionEstado.Abierta}");

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Caja>()
            .WithMany()
            .HasForeignKey(x => x.CajaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Sucursal>()
            .WithMany()
            .HasForeignKey(x => x.SucursalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
