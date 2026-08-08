using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface IAuditoriaServicio
{
    Task RegistrarAsync(string accion, string tabla, string descripcion);
    Task<List<AuditoriaDto>> ObtenerTodosAsync();
    Task<List<AuditoriaDto>> ObtenerPorTablaAsync(string tabla);
}
