using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class NotificacionApiServicio : ApiServicioBase, INotificacionApiServicio
{
    public NotificacionApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<List<NotificacionViewModel>> ObtenerPorUsuarioAsync(string usuario)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/notificaciones/usuario/{usuario}");
        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return new List<NotificacionViewModel>();
        }
        return await LeerRespuestaExitosaAsync<List<NotificacionViewModel>>(respuesta);
    }

    public async Task MarcarComoLeidaAsync(int notificacionId)
    {
        var respuesta = await ClienteHttp.PatchAsync($"api/notificaciones/{notificacionId}/marcar-leida", null);
        await AsegurarExitoAsync(respuesta);
    }
}
