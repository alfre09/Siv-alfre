using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface IEstadoVueloApiServicio
{
    Task<List<EstadoVueloModelo>> ObtenerTodosAsync();
}
