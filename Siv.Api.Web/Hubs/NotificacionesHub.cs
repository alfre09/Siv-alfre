using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Siv.Api.Web.Hubs;

// [Authorize]
public class NotificacionesHub : Hub
{
    // Opcional: Agregar métodos para gestionar grupos si quisiéramos enviar notificaciones por vuelo específico
    public async Task SuscribirseAVuelo(int vueloId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Vuelo_{vueloId}");
    }

    public async Task Identificarse(string usuario)
    {
        if (!string.IsNullOrWhiteSpace(usuario))
            await Groups.AddToGroupAsync(Context.ConnectionId, usuario);
    }
}
