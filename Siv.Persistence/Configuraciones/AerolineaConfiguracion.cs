using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class AerolineaConfiguracion : IEntityTypeConfiguration<Aerolinea>
{
    public void Configure(EntityTypeBuilder<Aerolinea> builder)
    {
        builder.ToTable("Aerolineas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("AerolineaId")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Codigo)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(a => a.Codigo)
            .IsUnique();
    }
}
