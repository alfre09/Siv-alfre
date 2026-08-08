using System.Net;
using System.Text.Json;
using Siv.Application.Excepciones;
using Siv.Domain.Excepciones;

namespace Siv.Api.Desktop.Middleware;

public class MiddlewareManejoDeExcepciones
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<MiddlewareManejoDeExcepciones> _logger;

    public MiddlewareManejoDeExcepciones(RequestDelegate siguiente, ILogger<MiddlewareManejoDeExcepciones> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (Exception excepcion)
        {
            await ManejarExcepcionAsync(contexto, excepcion);
        }
    }

    private async Task ManejarExcepcionAsync(HttpContext contexto, Exception excepcion)
    {
        var (codigoEstado, mensaje) = excepcion switch
        {
            ExcepcionRecursoNoEncontrado => (HttpStatusCode.NotFound, excepcion.Message),
            ExcepcionDeValidacion => (HttpStatusCode.BadRequest, excepcion.Message),
            ExcepcionDeDominio => (HttpStatusCode.BadRequest, excepcion.Message),
            ArgumentException => (HttpStatusCode.BadRequest, excepcion.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado al procesar la solicitud.")
        };

        if (codigoEstado == HttpStatusCode.InternalServerError)
            _logger.LogError(excepcion, "Error no controlado en la API");

        contexto.Response.ContentType = "application/json";
        contexto.Response.StatusCode = (int)codigoEstado;

        var respuesta = JsonSerializer.Serialize(new
        {
            estado = (int)codigoEstado,
            mensaje
        });

        await contexto.Response.WriteAsync(respuesta);
    }
}
