using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class CambioOperativoRepositorio : RepositorioBase<CambioOperativo>, ICambioOperativoRepositorio
{
    public CambioOperativoRepositorio(SivDbContext contexto, ILogger<CambioOperativoRepositorio> logger) : base(contexto, logger)
    {
    }

    public async Task<List<CambioOperativo>> ObtenerPorVueloAsync(int vueloId)
    {
        _logger.LogInformation("Obteniendo cambios operativos para el vuelo: {VueloId}", vueloId);
        return await ConjuntoDatos
            .Where(c => c.VueloId == vueloId)
            .ToListAsync();
    }
}
