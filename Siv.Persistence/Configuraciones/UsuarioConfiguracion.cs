using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Configuraciones;

public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("UsuarioId")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.NombreUsuario)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(300)
            .HasDefaultValue(string.Empty);

        builder.Property(u => u.Activo)
            .IsRequired();

        builder.Property(u => u.FechaCreacion)
            .IsRequired();

        builder.HasIndex(u => u.NombreUsuario)
            .IsUnique();
    }
}
