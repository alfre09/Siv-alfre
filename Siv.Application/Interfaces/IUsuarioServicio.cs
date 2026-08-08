using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface IUsuarioServicio
{
    Task RegistrarAsync(RegistrarUsuarioDto dto);
    Task<UsuarioAutenticacionDto?> ValidarCredencialesAsync(string nombreUsuario, string password);
}
