using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface IHistorialEstadoVueloApiServicio
{
    Task<List<HistorialEstadoVueloModelo>> ObtenerPorVueloAsync(int vueloId);
}
