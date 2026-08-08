using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface INotificacionApiServicio
{
    Task<List<NotificacionModelo>> ObtenerTodosAsync();
    Task<List<NotificacionModelo>> ObtenerPorUsuarioAsync(string usuario);
    Task<List<NotificacionModelo>> ObtenerPorVueloAsync(int vueloId);
    Task MarcarComoLeidaAsync(int id);
}
