using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface ISeguimientoApiServicio
{
    Task<List<SeguimientoModelo>> ObtenerTodosAsync();
    Task<SeguimientoModelo?> ObtenerPorIdAsync(int id);
    Task<SeguimientoModelo> CrearAsync(CrearSeguimientoModelo modelo);
    Task ActualizarAsync(ActualizarSeguimientoModelo modelo);
    Task EliminarAsync(int id);
}
