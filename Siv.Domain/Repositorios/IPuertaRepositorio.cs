using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IPuertaRepositorio
{
    Task<List<Puerta>> ObtenerTodosAsync();
    Task<Puerta?> ObtenerPorNombreAsync(string nombre);
}
