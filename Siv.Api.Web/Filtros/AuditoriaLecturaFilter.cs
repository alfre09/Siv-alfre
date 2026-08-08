using Microsoft.AspNetCore.Mvc.Filters;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Filtros;

public class AuditoriaLecturaFilter : IAsyncActionFilter
{
    private readonly IAuditoriaServicio _auditoriaServicio;

    public AuditoriaLecturaFilter(IAuditoriaServicio auditoriaServicio)
    {
        _auditoriaServicio = auditoriaServicio;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        // Solo auditar lecturas exitosas (GET)
        if (context.HttpContext.Request.Method == "GET" && 
            resultContext.Exception == null && 
            context.HttpContext.Response.StatusCode >= 200 && 
            context.HttpContext.Response.StatusCode < 300)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "Desconocido";
            var requestPath = context.HttpContext.Request.Path;
            var queryString = context.HttpContext.Request.QueryString;

            await _auditoriaServicio.RegistrarAsync(
                "LECTURA",
                controllerName,
                $"Consulta de lectura realizada. Path: {requestPath}{queryString}"
            );
        }
    }
}
