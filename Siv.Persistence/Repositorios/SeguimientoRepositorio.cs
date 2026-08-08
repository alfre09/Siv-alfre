using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class SeguimientoRepositorio : RepositorioBase<Seguimiento>, ISeguimientoRepositorio
{
    public SeguimientoRepositorio(SivDbContext contexto, ILogger<SeguimientoRepositorio> logger) : base(contexto, logger)
    {
    }

    public async Task<bool> ExisteSeguimientoAsync(int vueloId, string usuario)
    {
        _logger.LogInformation("Verificando existencia de seguimiento para el vuelo {VueloId} y usuario {Usuario}", vueloId, usuario);
        return await ConjuntoDatos
            .AnyAsync(s => s.VueloId == vueloId && s.Usuario == usuario);
    }

    public async Task<Seguimiento?> ObtenerPorVueloYUsuarioAsync(int vueloId, string usuario)
    {
        return await ConjuntoDatos.FirstOrDefaultAsync(
            s => s.VueloId == vueloId && s.Usuario == usuario);
    }

    public async Task<List<string>> ObtenerUsuariosInteresadosAsync(int vueloId)
    {
        _logger.LogInformation("Obteniendo usuarios interesados para el vuelo: {VueloId}", vueloId);
        return await ConjuntoDatos
            .Where(s => s.VueloId == vueloId)
            .Select(s => s.Usuario)
            .Distinct()
            .ToListAsync();
    }
}
