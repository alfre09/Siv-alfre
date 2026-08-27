using Microsoft.Extensions.DependencyInjection;
using Siv.Application.Interfaces;
using Siv.Application.Servicios;

namespace Siv.Application.Configuracion;

public static class DependenciasApplication
{
    public static IServiceCollection AgregarServiciosDeAplicacion(this IServiceCollection servicios)
    {
        servicios.AddScoped<IAuditoriaServicio, AuditoriaServicio>();
        servicios.AddScoped<IAerolineaServicio, AerolineaServicio>();
        servicios.AddScoped<IAeropuertoServicio, AeropuertoServicio>();
        servicios.AddScoped<IEstadoVueloServicio, EstadoVueloServicio>();
        servicios.AddScoped<IVueloServicio, VueloServicio>();
        servicios.AddScoped<IReservaServicio, ReservaServicio>();
        servicios.AddScoped<IUsuarioServicio, UsuarioServicio>();
        servicios.AddScoped<ISeguimientoServicio, SeguimientoServicio>();
        servicios.AddScoped<IHistorialEstadoVueloServicio, HistorialEstadoVueloServicio>();
        servicios.AddScoped<INotificacionServicio, NotificacionServicio>();
        servicios.AddScoped<IEmailServicio, SmtpEmailServicio>();
        servicios.AddScoped<ICambioOperativoServicio, CambioOperativoServicio>();
        return servicios;
    }
}
