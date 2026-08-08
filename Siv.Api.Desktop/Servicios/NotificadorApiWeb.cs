using System.Net.Http.Json;
using Siv.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Siv.Api.Desktop.Servicios;

public class NotificadorApiWeb : INotificadorTiempoReal
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificadorApiWeb> _logger;

    public NotificadorApiWeb(HttpClient httpClient, ILogger<NotificadorApiWeb> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task EnviarNotificacionAsync(string usuario, string mensaje)
    {
        try
        {
            var payload = new { Usuario = usuario, Mensaje = mensaje };
            await _httpClient.PostAsJsonAsync("api/notificaciones/difundir", payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación a la API Web.");
        }
    }

    public async Task EnviarNotificacionGeneralAsync(string mensaje)
    {
        try
        {
            var payload = new { Usuario = string.Empty, Mensaje = mensaje };
            await _httpClient.PostAsJsonAsync("api/notificaciones/difundir", payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación general a la API Web.");
        }
    }
}
