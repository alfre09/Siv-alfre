using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface IReservaServicio
{
    Task<ReservaDto> CrearAsync(int vueloId, string usuario);
    Task<List<ReservaDto>> ObtenerPorUsuarioAsync(string usuario);
    Task CancelarAsync(int reservaId, string usuario);
}
