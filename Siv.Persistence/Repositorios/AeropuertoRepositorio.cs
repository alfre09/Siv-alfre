using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class AeropuertoRepositorio : RepositorioBase<Aeropuerto>, IAeropuertoRepositorio
{
    public AeropuertoRepositorio(SivDbContext contexto, ILogger<AeropuertoRepositorio> logger) : base(contexto, logger)
    {
        _logger.LogInformation("AeropuertoRepositorio inicializado");
    }

    public async Task<bool> ExisteCodigoAsync(string codigo)
    {
        _logger.LogInformation("Verificando si existe el código de aeropuerto: {Codigo}", codigo);
        return await ConjuntoDatos
            .AnyAsync(a => a.Codigo.ToUpper() == codigo.ToUpper());
    }

    public async Task<bool> TieneVuelosAsociadosAsync(int aeropuertoId)
    {
        _logger.LogInformation("Verificando si el aeropuerto {AeropuertoId} tiene vuelos asociados", aeropuertoId);
        return await Contexto.Vuelos
            .AnyAsync(v => v.AeropuertoOrigenId == aeropuertoId || v.AeropuertoDestinoId == aeropuertoId);
    }
}
