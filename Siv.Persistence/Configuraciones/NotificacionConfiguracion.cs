using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class NotificacionConfiguracion : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("Notificaciones");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("NotificacionId")
            .ValueGeneratedOnAdd();

        builder.Property(n => n.Usuario)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(n => n.Mensaje)
            .IsRequired();

        builder.Property(n => n.Leida)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(n => n.FechaEnvio)
            .IsRequired();

        builder.HasOne(n => n.Vuelo)
            .WithMany()
            .HasForeignKey(n => n.VueloId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.CambioOperativo)
            .WithMany()
            .HasForeignKey(n => n.CambioOperativoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => n.CambioOperativoId);
    }
}
