using Siv.Domain.Excepciones;

namespace Siv.Domain.Entidades;

public class Usuario : EntidadBase
{
    protected Usuario()
    {
        NombreUsuario = string.Empty;
        Rol = string.Empty;
        PasswordHash = string.Empty;
    }

    public Usuario(string nombreUsuario, string rol, string? correo = null, bool activo = true)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario))
            throw new ArgumentException("El nombre de usuario es obligatorio.", nameof(nombreUsuario));

        if (string.IsNullOrWhiteSpace(rol))
            throw new ArgumentException("El rol es obligatorio.", nameof(rol));

        NombreUsuario = nombreUsuario.Trim().ToLower();
        Rol = rol.Trim();
        PasswordHash = string.Empty;
        Correo = correo?.Trim().ToLowerInvariant();
        Activo = activo;
        FechaCreacion = DateTime.UtcNow;
    }

    public string NombreUsuario { get; private set; }
    public string Rol { get; private set; }
    public string PasswordHash { get; private set; }
    public string? Correo { get; private set; }
    public bool Activo { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    public void EstablecerPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("La contraseña debe tener un hash válido.", nameof(passwordHash));

        PasswordHash = passwordHash;
    }
}
