using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class EstadoVueloRepositorio : RepositorioBase<EstadoVuelo>, IEstadoVueloRepositorio
{
    public EstadoVueloRepositorio(SivDbContext contexto, ILogger<EstadoVueloRepositorio> logger) : base(contexto, logger)
    {
    }

    public async Task<EstadoVuelo?> ObtenerPorNombreAsync(string nombre)
    {
        _logger.LogInformation("Obteniendo estado de vuelo por nombre: {Nombre}", nombre);
        return await ConjuntoDatos
            .FirstOrDefaultAsync(e => e.Nombre == nombre);
    }
}
