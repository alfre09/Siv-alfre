using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class HistorialEstadoVueloRepositorio : RepositorioBase<HistorialEstadoVuelo>, IHistorialEstadoVueloRepositorio
{
    public HistorialEstadoVueloRepositorio(SivDbContext contexto, ILogger<HistorialEstadoVueloRepositorio> logger) : base(contexto, logger)
    {
    }

    public async Task<List<HistorialEstadoVuelo>> ObtenerPorVueloAsync(int vueloId)
    {
        _logger.LogInformation("Obteniendo historial de estados para el vuelo: {VueloId}", vueloId);
        return await ConjuntoDatos
            .Where(h => h.VueloId == vueloId)
            .ToListAsync();
    }
}
