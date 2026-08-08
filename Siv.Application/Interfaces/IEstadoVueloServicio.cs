using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface IEstadoVueloServicio
{
    Task<List<EstadoVueloDto>> ObtenerTodosAsync();
}
