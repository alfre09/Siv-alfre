using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class AuditoriaConfiguracion : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        builder.ToTable("Auditorias");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("AuditoriaId")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Accion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Tabla)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Descripcion)
            .IsRequired();

        builder.Property(a => a.Fecha)
            .IsRequired();
    }
}
