namespace Siv.Application.Dtos;

public class RegistrarUsuarioDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UsuarioAutenticacionDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
