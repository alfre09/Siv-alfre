using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IVueloRepositorio : IRepositorioBase<Vuelo>
{
    Task<Vuelo?> ObtenerConDetalleAsync(int id);
    Task<List<Vuelo>> ObtenerTodosConDetalleAsync();
    Task<List<Vuelo>> ObtenerPorAerolineaAsync(int aerolineaId);
    Task<List<Vuelo>> ObtenerPorEstadoAsync(int estadoVueloId);
    Task<bool> ExisteNumeroVueloAsync(string numeroVuelo, int? excluirVueloId = null);
}
