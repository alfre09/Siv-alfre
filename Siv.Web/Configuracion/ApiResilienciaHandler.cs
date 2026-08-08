using System.Net;
using System.Net.Http.Json;

namespace Siv.Web.Configuracion;

public sealed class ApiResilienciaHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return CrearRespuestaError(HttpStatusCode.GatewayTimeout, "La API tardó demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            return CrearRespuestaError(HttpStatusCode.ServiceUnavailable,
                "La API no está disponible. Verifica que el servicio esté iniciado.");
        }
    }

    private static HttpResponseMessage CrearRespuestaError(HttpStatusCode estado, string mensaje) =>
        new(estado)
        {
            Content = JsonContent.Create(new { mensaje })
        };
}
