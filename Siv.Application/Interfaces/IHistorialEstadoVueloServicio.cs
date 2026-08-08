using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface IHistorialEstadoVueloServicio
{
    Task<List<HistorialEstadoVueloDto>> ObtenerPorVueloAsync(int vueloId);
    Task RegistrarCambioDeEstadoAsync(int vueloId, int estadoAnteriorId, int estadoNuevoId);
}
