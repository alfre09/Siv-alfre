using Siv.Application.Dtos;

namespace Siv.Application.Interfaces;

public interface INotificacionServicio
{
    Task<List<NotificacionDto>> ObtenerTodosAsync();
    Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(string usuario);
    Task<List<NotificacionDto>> ObtenerPorVueloAsync(int vueloId);
    Task MarcarComoLeidaAsync(int notificacionId, string usuario);
    Task GenerarNotificacionesPorCambioAsync(int vueloId, int cambioOperativoId, string mensaje);
}
