using Microsoft.AspNetCore.SignalR;
using Siv.Api.Web.Hubs;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Servicios;

public class NotificadorSignalR : INotificadorTiempoReal
{
    private readonly IHubContext<NotificacionesHub> _hubContext;

    public NotificadorSignalR(IHubContext<NotificacionesHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task EnviarNotificacionAsync(string usuario, string mensaje)
    {
        // En SignalR podemos enviar a un usuario específico si tenemos configurado un CustomUserIdProvider
        // Por simplicidad, enviaremos a un grupo con el nombre del usuario
        await _hubContext.Clients.Group(usuario).SendAsync("RecibirNotificacion", mensaje);
    }

    public async Task EnviarNotificacionGeneralAsync(string mensaje)
    {
        await _hubContext.Clients.All.SendAsync("RecibirNotificacion", mensaje);
    }
}
