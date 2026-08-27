using System.Net.Http.Json;
using System.Net.Http.Headers;
using Siv.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Siv.Api.Desktop.Servicios;

public class NotificadorApiWeb : INotificadorTiempoReal
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<NotificadorApiWeb> _logger;

    public NotificadorApiWeb(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<NotificadorApiWeb> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<bool> EnviarNotificacionAsync(string usuario, string mensaje)
    {
        try
        {
            var payload = new { Usuario = usuario, Mensaje = mensaje };
            using var solicitud = CrearSolicitud(payload);
            using var respuesta = await _httpClient.SendAsync(solicitud);
            if (!respuesta.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "La API Web rechazó la notificación para {Usuario}. Código HTTP: {StatusCode}",
                    usuario,
                    respuesta.StatusCode);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación a la API Web.");
            return false;
        }
    }

    public async Task<bool> EnviarNotificacionGeneralAsync(string mensaje)
    {
        try
        {
            var payload = new { Usuario = string.Empty, Mensaje = mensaje };
            using var solicitud = CrearSolicitud(payload);
            using var respuesta = await _httpClient.SendAsync(solicitud);
            if (!respuesta.IsSuccessStatusCode)
            {
                _logger.LogWarning("La API Web rechazó la notificación general. Código HTTP: {StatusCode}", respuesta.StatusCode);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación general a la API Web.");
            return false;
        }
    }

    private HttpRequestMessage CrearSolicitud(object payload)
    {
        var solicitud = new HttpRequestMessage(HttpMethod.Post, "api/notificaciones/difundir")
        {
            Content = JsonContent.Create(payload)
        };

        var autorizacion = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(autorizacion) &&
            AuthenticationHeaderValue.TryParse(autorizacion, out var encabezado))
        {
            solicitud.Headers.Authorization = encabezado;
        }

        return solicitud;
    }
}
