using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface IVueloServicio
{
    Task<List<VueloDto>> ObtenerTodosAsync(string? rolUsuario = null);
    Task<List<VueloDto>> ObtenerDisponiblesAsync(int? origenId = null, int? destinoId = null, DateTime? fecha = null);
    Task<VueloDto?> ObtenerPorIdAsync(int id, string? rolUsuario = null);
    Task<VueloDto> CrearAsync(CrearVueloDto dto);
    Task ActualizarAsync(ActualizarVueloDto dto, string? rolUsuario = null);
    Task EliminarAsync(int id);
}
