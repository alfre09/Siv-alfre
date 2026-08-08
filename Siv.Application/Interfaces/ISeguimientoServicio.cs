using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface ISeguimientoServicio
{
    Task<List<SeguimientoDto>> ObtenerTodosAsync();
    Task<SeguimientoDto?> ObtenerPorIdAsync(int id);
    Task<SeguimientoDto> CrearAsync(CrearSeguimientoDto dto);
    Task ActualizarAsync(ActualizarSeguimientoDto dto);
    Task EliminarAsync(int id);
    Task<SeguimientoDto?> ObtenerPorVueloYUsuarioAsync(int vueloId, string usuario);
    Task<List<SeguimientoDto>> ObtenerPorUsuarioAsync(string usuario);
}
