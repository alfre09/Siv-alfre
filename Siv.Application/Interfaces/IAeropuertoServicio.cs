using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface IAeropuertoServicio
{
    Task<List<AeropuertoDto>> ObtenerTodosAsync();
    Task<AeropuertoDto?> ObtenerPorIdAsync(int id);
    Task<AeropuertoDto> CrearAsync(CrearAeropuertoDto dto);
    Task ActualizarAsync(ActualizarAeropuertoDto dto);
    Task EliminarAsync(int id);
}
