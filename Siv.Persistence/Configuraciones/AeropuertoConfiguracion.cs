using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class AeropuertoConfiguracion : IEntityTypeConfiguration<Aeropuerto>
{
    public void Configure(EntityTypeBuilder<Aeropuerto> builder)
    {
        builder.ToTable("Aeropuertos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("AeropuertoId")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Codigo)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Ciudad)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Pais)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(a => a.Codigo)
            .IsUnique();
    }
}
