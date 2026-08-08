using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class HistorialEstadoVueloConfiguracion : IEntityTypeConfiguration<HistorialEstadoVuelo>
{
    public void Configure(EntityTypeBuilder<HistorialEstadoVuelo> builder)
    {
        builder.ToTable("HistorialEstados");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("HistorialEstadoVueloId")
            .ValueGeneratedOnAdd();

        builder.Property(h => h.EstadoAnteriorId)
            .IsRequired();

        builder.Property(h => h.EstadoNuevoId)
            .IsRequired();

        builder.Property(h => h.FechaCambio)
            .IsRequired();

        builder.HasOne(h => h.Vuelo)
            .WithMany()
            .HasForeignKey(h => h.VueloId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
