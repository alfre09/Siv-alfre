using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IUsuarioRepositorio : IRepositorioBase<Usuario>
{
    Task<Usuario?> ObtenerPorNombreAsync(string nombreUsuario);
}
