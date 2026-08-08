using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface IVueloApiServicio
{
    Task<List<VueloModelo>> ObtenerTodosAsync();
    Task<VueloModelo?> ObtenerPorIdAsync(int id);
    Task<VueloModelo> CrearAsync(CrearVueloModelo modelo);
    Task ActualizarAsync(ActualizarVueloModelo modelo);
    Task EliminarAsync(int id);
}
