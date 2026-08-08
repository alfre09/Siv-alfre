using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class SeguimientoConfiguracion : IEntityTypeConfiguration<Seguimiento>
{
    public void Configure(EntityTypeBuilder<Seguimiento> builder)
    {
        builder.ToTable("Seguimientos");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("SeguimientoId")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Usuario)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.FechaSeguimiento)
            .IsRequired();

        builder.HasIndex(s => new { s.VueloId, s.Usuario })
            .IsUnique();

        builder.HasOne(s => s.Vuelo)
            .WithMany()
            .HasForeignKey(s => s.VueloId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
