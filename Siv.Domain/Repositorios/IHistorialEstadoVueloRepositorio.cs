using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IHistorialEstadoVueloRepositorio : IRepositorioBase<HistorialEstadoVuelo>
{
    Task<List<HistorialEstadoVuelo>> ObtenerPorVueloAsync(int vueloId);
}
