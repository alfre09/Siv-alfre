using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Servicios;

/// <summary>
/// Implementación nula del notificador en tiempo real para la API Desktop.
/// El Desktop no usa SignalR, así que las notificaciones en tiempo real se omiten.
/// </summary>
public class NotificadorNulo : INotificadorTiempoReal
{
    public Task EnviarNotificacionAsync(string usuario, string mensaje)
    {
        // No-op: El cliente Desktop no soporta notificaciones en tiempo real.
        return Task.CompletedTask;
    }

    public Task EnviarNotificacionGeneralAsync(string mensaje)
    {
        // No-op: El cliente Desktop no soporta notificaciones en tiempo real.
        return Task.CompletedTask;
    }
}
