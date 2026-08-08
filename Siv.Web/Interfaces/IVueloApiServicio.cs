using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface IVueloApiServicio
{
    Task<List<VueloViewModel>> ObtenerTodosAsync();
    Task<List<VueloViewModel>> ObtenerDisponiblesAsync(int? origenId = null, int? destinoId = null, DateTime? fecha = null);
    Task<VueloViewModel?> ObtenerPorIdAsync(int id);
}
