using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface IAeropuertoApiServicio
{
    Task<List<AeropuertoModelo>> ObtenerTodosAsync();
    Task<AeropuertoModelo?> ObtenerPorIdAsync(int id);
    Task<AeropuertoModelo> CrearAsync(CrearAeropuertoModelo modelo);
    Task ActualizarAsync(ActualizarAeropuertoModelo modelo);
    Task EliminarAsync(int id);
}
