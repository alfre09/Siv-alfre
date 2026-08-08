using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class VueloRepositorio : RepositorioBase<Vuelo>, IVueloRepositorio
{
    public VueloRepositorio(SivDbContext contexto, ILogger<VueloRepositorio> logger) : base(contexto, logger)
    {
    }

    public async Task<Vuelo?> ObtenerConDetalleAsync(int id)
    {
        _logger.LogInformation("Obteniendo vuelo con detalle para ID: {Id}", id);
        return await ConjuntoDatos
            .Include(v => v.Aerolinea)
            .Include(v => v.AeropuertoOrigen)
            .Include(v => v.AeropuertoDestino)
            .Include(v => v.EstadoVuelo)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<List<Vuelo>> ObtenerTodosConDetalleAsync()
    {
        _logger.LogInformation("Obteniendo todos los vuelos con detalle");
        return await ConjuntoDatos
            .Include(v => v.Aerolinea)
            .Include(v => v.AeropuertoOrigen)
            .Include(v => v.AeropuertoDestino)
            .Include(v => v.EstadoVuelo)
            .OrderByDescending(v => v.HorarioProgramado)
            .ToListAsync();
    }

    public async Task<List<Vuelo>> ObtenerPorAerolineaAsync(int aerolineaId)
    {
        _logger.LogInformation("Obteniendo vuelos para la aerolínea: {AerolineaId}", aerolineaId);
        return await ConjuntoDatos
            .Where(v => v.AerolineaId == aerolineaId)
            .ToListAsync();
    }

    public async Task<List<Vuelo>> ObtenerPorEstadoAsync(int estadoVueloId)
    {
        _logger.LogInformation("Obteniendo vuelos para el estado: {EstadoVueloId}", estadoVueloId);
        return await ConjuntoDatos
            .Where(v => v.EstadoVueloId == estadoVueloId)
            .ToListAsync();
    }

    public async Task<bool> ExisteNumeroVueloAsync(string numeroVuelo, int? excluirVueloId = null)
    {
        _logger.LogInformation("Verificando si existe el número de vuelo: {NumeroVuelo}", numeroVuelo);
        var consulta = ConjuntoDatos.Where(v => v.NumeroVuelo == numeroVuelo);

        if (excluirVueloId.HasValue)
            consulta = consulta.Where(v => v.Id != excluirVueloId.Value);

        return await consulta.AnyAsync();
    }
}
