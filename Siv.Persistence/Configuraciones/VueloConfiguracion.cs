using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class VueloConfiguracion : IEntityTypeConfiguration<Vuelo>
{
    public void Configure(EntityTypeBuilder<Vuelo> builder)
    {
        builder.ToTable("Vuelos");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasColumnName("VueloId")
            .ValueGeneratedOnAdd();

        builder.Property(v => v.NumeroVuelo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.Puerta)
            .HasMaxLength(10);

        builder.Property(v => v.HorarioProgramado)
            .IsRequired();

        builder.Property(v => v.FechaCreacion)
            .IsRequired();

        builder.HasIndex(v => v.NumeroVuelo)
            .IsUnique();

        builder.Property(v => v.NivelVisibilidad)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Siv.Domain.Enums.NivelVisibilidad.Publico);

        builder.HasOne(v => v.Aerolinea)
            .WithMany()
            .HasForeignKey(v => v.AerolineaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.AeropuertoOrigen)
            .WithMany()
            .HasForeignKey(v => v.AeropuertoOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.AeropuertoDestino)
            .WithMany()
            .HasForeignKey(v => v.AeropuertoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.EstadoVuelo)
            .WithMany()
            .HasForeignKey(v => v.EstadoVueloId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
