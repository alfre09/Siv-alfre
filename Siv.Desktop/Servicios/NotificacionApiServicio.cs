using System.Net.Http;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class NotificacionApiServicio : ApiServicioBase, INotificacionApiServicio
{
    public NotificacionApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<NotificacionModelo>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/notificaciones");
        return await LeerRespuestaExitosaAsync<List<NotificacionModelo>>(respuesta);
    }

    public async Task<List<NotificacionModelo>> ObtenerPorUsuarioAsync(string usuario)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/notificaciones/usuario/{Uri.EscapeDataString(usuario)}");
        return await LeerRespuestaExitosaAsync<List<NotificacionModelo>>(respuesta);
    }

    public async Task<List<NotificacionModelo>> ObtenerPorVueloAsync(int vueloId)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/notificaciones/vuelo/{vueloId}");
        return await LeerRespuestaExitosaAsync<List<NotificacionModelo>>(respuesta);
    }

    public async Task MarcarComoLeidaAsync(int id)
    {
        var respuesta = await ClienteHttp.PatchAsync($"api/notificaciones/{id}/marcar-leida", null);
        await AsegurarExitoAsync(respuesta);
    }
}
