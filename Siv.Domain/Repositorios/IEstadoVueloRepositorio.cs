using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IEstadoVueloRepositorio : IRepositorioBase<EstadoVuelo>
{
    Task<EstadoVuelo?> ObtenerPorNombreAsync(string nombre);
}
