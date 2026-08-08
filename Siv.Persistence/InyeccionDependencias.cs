using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siv.Application.Interfaces;
using Siv.Application.Servicios;
using Siv.Domain.Repositorios;
using Siv.Persistence.Repositorios;

namespace Siv.Persistence;

public static class InyeccionDependencias
{
    public static IServiceCollection AddSIVDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Base de Datos
        services.AddDbContext<SivDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // 2. Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 3. Repositorios
        services.AddScoped<IAerolineaRepositorio, AerolineaRepositorio>();
        services.AddScoped<IAeropuertoRepositorio, AeropuertoRepositorio>();
        services.AddScoped<IAuditoriaRepositorio, AuditoriaRepositorio>();
        services.AddScoped<ICambioOperativoRepositorio, CambioOperativoRepositorio>();
        services.AddScoped<IEstadoVueloRepositorio, EstadoVueloRepositorio>();
        services.AddScoped<IHistorialEstadoVueloRepositorio, HistorialEstadoVueloRepositorio>();
        services.AddScoped<INotificacionRepositorio, NotificacionRepositorio>();
        services.AddScoped<ISeguimientoRepositorio, SeguimientoRepositorio>();
        services.AddScoped<IReservaRepositorio, ReservaRepositorio>();
        services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
        services.AddScoped<IVueloRepositorio, VueloRepositorio>();

        // 4. Servicios de Aplicación
        services.AddScoped<IAerolineaServicio, AerolineaServicio>();
        services.AddScoped<IAeropuertoServicio, AeropuertoServicio>();
        services.AddScoped<IAuditoriaServicio, AuditoriaServicio>();
        services.AddScoped<ICambioOperativoServicio, CambioOperativoServicio>();
        services.AddScoped<IEstadoVueloServicio, EstadoVueloServicio>();
        services.AddScoped<IHistorialEstadoVueloServicio, HistorialEstadoVueloServicio>();
        services.AddScoped<INotificacionServicio, NotificacionServicio>();
        services.AddScoped<ISeguimientoServicio, SeguimientoServicio>();
        services.AddScoped<IVueloServicio, VueloServicio>();
        services.AddScoped<IReservaServicio, ReservaServicio>();
        services.AddScoped<IUsuarioServicio, UsuarioServicio>();
        return services;
    }
}
