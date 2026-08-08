using System.Net.Http;

namespace Siv.Desktop.Servicios;

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
            throw new ExcepcionApi(504, "La API tardó demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new ExcepcionApi(503, "La API Desktop no está disponible. Verifica que el servicio esté iniciado.");
        }
    }
}
