using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface IAuditoriaApiServicio
{
    Task<List<AuditoriaModelo>> ObtenerTodosAsync();
    Task<List<AuditoriaModelo>> ObtenerPorTablaAsync(string tabla);
}
