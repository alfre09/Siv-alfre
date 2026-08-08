using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface IEstadoVueloApiServicio
{
    Task<List<EstadoVueloViewModel>> ObtenerTodosAsync();
}
