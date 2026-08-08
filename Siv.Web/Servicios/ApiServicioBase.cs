using System.Net.Http.Json;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public abstract class ApiServicioBase
{
    protected readonly HttpClient ClienteHttp;

    protected ApiServicioBase(HttpClient clienteHttp)
    {
        ClienteHttp = clienteHttp;
    }

    protected async Task<T> LeerRespuestaExitosaAsync<T>(HttpResponseMessage respuesta)
    {
        if (!respuesta.IsSuccessStatusCode)
            throw await ConstruirExcepcionAsync(respuesta);

        var contenido = await respuesta.Content.ReadFromJsonAsync<T>();

        if (contenido is null)
            throw new ExcepcionApi((int)respuesta.StatusCode, "La API no devolvió información.");

        return contenido;
    }

    protected async Task AsegurarExitoAsync(HttpResponseMessage respuesta)
    {
        if (!respuesta.IsSuccessStatusCode)
            throw await ConstruirExcepcionAsync(respuesta);
    }

    private static async Task<ExcepcionApi> ConstruirExcepcionAsync(HttpResponseMessage respuesta)
    {
        try
        {
            var error = await respuesta.Content.ReadFromJsonAsync<RespuestaErrorApiViewModel>();

            if (error is not null && !string.IsNullOrWhiteSpace(error.Mensaje))
                return new ExcepcionApi((int)respuesta.StatusCode, error.Mensaje);
        }
        catch
        {
            // El cuerpo de la respuesta no tenía el formato de error esperado.
        }

        return new ExcepcionApi(
            (int)respuesta.StatusCode,
            "No se pudo completar la solicitud con la API. Código de estado: " + respuesta.StatusCode);
    }
}
