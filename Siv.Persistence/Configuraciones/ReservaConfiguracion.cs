using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class ReservaConfiguracion : IEntityTypeConfiguration<Reserva>
{
    public void Configure(EntityTypeBuilder<Reserva> builder)
    {
        builder.ToTable("Reservas");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("ReservaId")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Usuario)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.FechaReserva).IsRequired();
        builder.Property(r => r.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(r => new { r.Usuario, r.VueloId, r.Estado });

        builder.HasOne(r => r.Vuelo)
            .WithMany()
            .HasForeignKey(r => r.VueloId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
