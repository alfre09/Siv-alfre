using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class AerolineaRepositorio : RepositorioBase<Aerolinea>, IAerolineaRepositorio
{
    public AerolineaRepositorio(SivDbContext contexto, ILogger<AerolineaRepositorio> logger) : base(contexto, logger)
    {
        _logger.LogInformation("AerolineaRepositorio inicializado");
    }

    public async Task<bool> ExisteCodigoAsync(string codigo)
    {
        _logger.LogInformation("Verificando si existe el código de aerolínea: {Codigo}", codigo);
        return await ConjuntoDatos
            .AnyAsync(a => a.Codigo.ToUpper() == codigo.ToUpper());
    }

    public async Task<bool> TieneVuelosAsociadosAsync(int aerolineaId)
    {
        _logger.LogInformation("Verificando si la aerolínea {AerolineaId} tiene vuelos asociados", aerolineaId);
        return await Contexto.Vuelos
            .AnyAsync(v => v.AerolineaId == aerolineaId);
    }
}
