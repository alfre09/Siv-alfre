using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class EstadoVueloServicio : IEstadoVueloServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly ILogger<EstadoVueloServicio> _logger;

    public EstadoVueloServicio(IUnitOfWork unidadDeTrabajo, ILogger<EstadoVueloServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _logger = logger;
    }

    public async Task<List<EstadoVueloDto>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo todos los estados de vuelo");
        var estados = await _unidadDeTrabajo.EstadosVuelo.ObtenerTodosAsync();
        return estados.Select(e => e.ADto()).ToList();
    }
}
