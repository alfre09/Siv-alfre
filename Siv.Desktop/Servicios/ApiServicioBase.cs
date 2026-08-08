using System.Net.Http;
using System.Net.Http.Json;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

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
            var error = await respuesta.Content.ReadFromJsonAsync<RespuestaErrorApi>();

            if (error is not null && !string.IsNullOrWhiteSpace(error.Mensaje))
                return new ExcepcionApi((int)respuesta.StatusCode, error.Mensaje);
        }
        catch
        {
        }

        var mensaje = respuesta.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "La sesión del Desktop expiró o el token no es válido. Cierra sesión y vuelve a iniciar sesión.",
            System.Net.HttpStatusCode.Forbidden => "Tu rol no tiene permisos para realizar esta operación.",
            System.Net.HttpStatusCode.NotFound => "El recurso solicitado no existe.",
            _ => "No se pudo completar la solicitud con el servicio de vuelos."
        };

        return new ExcepcionApi((int)respuesta.StatusCode, mensaje);
    }
}

public class ExcepcionApi : Exception
{
    public int CodigoEstado { get; }

    public ExcepcionApi(int codigoEstado, string mensaje) : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }
}
