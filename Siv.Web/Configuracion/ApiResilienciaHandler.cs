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
            throw new Siv.Web.Modelos.ExcepcionApi(
                504, "La API tardó demasiado en responder.");
        }
        catch (HttpRequestException)
        {
            throw new Siv.Web.Modelos.ExcepcionApi(
                503, "La API no está disponible. Verifica que el servicio esté iniciado.");
        }
    }
}
