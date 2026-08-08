using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface INotificacionApiServicio
{
    Task<List<NotificacionViewModel>> ObtenerPorUsuarioAsync(string usuario);
    Task MarcarComoLeidaAsync(int notificacionId);
}
