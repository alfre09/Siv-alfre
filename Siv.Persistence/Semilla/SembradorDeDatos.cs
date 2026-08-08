using Microsoft.EntityFrameworkCore;
using Siv.Domain.Entidades;

namespace Siv.Persistence.Semilla;

public static class SembradorDeDatos
{
    public static async Task InicializarAsync(SivDbContext contexto)
    {
        await SembrarEstadosAsync(contexto);
        await SembrarAerolineasAsync(contexto);
        await SembrarAeropuertosAsync(contexto);
        await SembrarUsuariosAsync(contexto);
    }

    private static async Task SembrarEstadosAsync(SivDbContext contexto)
    {
        var nombresRequeridos = EstadoVuelo.SecuenciaOperativa.Append(EstadoVuelo.Cancelado);

        foreach (var nombre in nombresRequeridos)
        {
            var existe = await contexto.EstadosVuelo.AnyAsync(e => e.Nombre == nombre);

            if (!existe)
                contexto.EstadosVuelo.Add(new EstadoVuelo(nombre));
        }

        await contexto.SaveChangesAsync();
    }

    private static async Task SembrarAerolineasAsync(SivDbContext contexto)
    {
        if (await contexto.Aerolineas.AnyAsync())
            return;

        contexto.Aerolineas.AddRange(
            new Aerolinea("AA", "American Airlines"),
            new Aerolinea("LA", "LATAM Airlines"),
            new Aerolinea("CM", "Copa Airlines"),
            new Aerolinea("B6", "JetBlue Airways")
        );

        await contexto.SaveChangesAsync();
    }

    private static async Task SembrarAeropuertosAsync(SivDbContext contexto)
    {
        if (await contexto.Aeropuertos.AnyAsync())
            return;

        contexto.Aeropuertos.AddRange(
            new Aeropuerto("SDQ", "Aeropuerto Las Américas", "Santo Domingo", "República Dominicana"),
            new Aeropuerto("PUJ", "Aeropuerto Internacional de Punta Cana", "Punta Cana", "República Dominicana"),
            new Aeropuerto("MIA", "Aeropuerto Internacional de Miami", "Miami", "Estados Unidos"),
            new Aeropuerto("JFK", "Aeropuerto John F. Kennedy", "Nueva York", "Estados Unidos"),
            new Aeropuerto("MAD", "Aeropuerto Adolfo Suárez Madrid-Barajas", "Madrid", "España")
        );

        await contexto.SaveChangesAsync();
    }

    private static async Task SembrarUsuariosAsync(SivDbContext contexto)
    {
        if (await contexto.Usuarios.AnyAsync())
            return;

        contexto.Usuarios.AddRange(
            new Usuario("admin", "Admin"),
            new Usuario("operador1", "Operador")
        );

        await contexto.SaveChangesAsync();
    }
}
