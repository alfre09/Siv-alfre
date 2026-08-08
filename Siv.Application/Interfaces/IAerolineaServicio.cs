using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface IAerolineaServicio
{
    Task<List<AerolineaDto>> ObtenerTodosAsync();
    Task<AerolineaDto?> ObtenerPorIdAsync(int id);
    Task<AerolineaDto> CrearAsync(CrearAerolineaDto dto);
    Task ActualizarAsync(ActualizarAerolineaDto dto);
    Task EliminarAsync(int id);
}
