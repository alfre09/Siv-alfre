using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface IAerolineaApiServicio
{
    Task<List<AerolineaModelo>> ObtenerTodosAsync();
    Task<AerolineaModelo?> ObtenerPorIdAsync(int id);
    Task<AerolineaModelo> CrearAsync(CrearAerolineaModelo modelo);
    Task ActualizarAsync(ActualizarAerolineaModelo modelo);
    Task EliminarAsync(int id);
}
