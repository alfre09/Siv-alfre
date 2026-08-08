using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class EstadoVueloConfiguracion : IEntityTypeConfiguration<EstadoVuelo>
{
    public void Configure(EntityTypeBuilder<EstadoVuelo> builder)
    {
        builder.ToTable("EstadosVuelo");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("EstadoVueloId")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.Nombre)
            .IsUnique();
    }
}
