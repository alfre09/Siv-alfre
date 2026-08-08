using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siv.Domain.Repositorios;
using Siv.Persistence.Repositorios;

namespace Siv.Persistence.Configuracion;

public static class DependenciasPersistence
{
    public static IServiceCollection AgregarPersistencia(
        this IServiceCollection servicios, IConfiguration configuracion)
    {
        servicios.AddDbContext<SivDbContext>(opciones =>
            opciones.UseSqlServer(configuracion.GetConnectionString("DefaultConnection")));

        servicios.AddScoped<IUnitOfWork, UnitOfWork>();

        return servicios;
    }
}
