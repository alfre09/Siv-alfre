using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class PuertaConfiguration : IEntityTypeConfiguration<Puerta>
{
    public void Configure(EntityTypeBuilder<Puerta> builder)
    {
        builder.ToTable("Puertas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(p => p.Aeropuerto)
            .WithMany()
            .HasForeignKey(p => p.AeropuertoId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
