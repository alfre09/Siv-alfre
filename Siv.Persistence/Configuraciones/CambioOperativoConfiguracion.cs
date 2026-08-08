using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class CambioOperativoConfiguracion : IEntityTypeConfiguration<CambioOperativo>
{
    public void Configure(EntityTypeBuilder<CambioOperativo> builder)
    {
        builder.ToTable("CambiosOperativos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("CambioOperativoId")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.TipoCambio)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Causa)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.ValorAnterior)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(c => c.ValorNuevo)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(c => c.FechaCambio)
            .IsRequired();

        builder.HasOne(c => c.Vuelo)
            .WithMany()
            .HasForeignKey(c => c.VueloId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
