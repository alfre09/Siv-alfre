using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class HistorialEstadoVueloServicio : IHistorialEstadoVueloServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly ILogger<HistorialEstadoVueloServicio> _logger;

    public HistorialEstadoVueloServicio(IUnitOfWork unidadDeTrabajo, ILogger<HistorialEstadoVueloServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _logger = logger;
    }

    public async Task<List<HistorialEstadoVueloDto>> ObtenerPorVueloAsync(int vueloId)
    {
        _logger.LogInformation("Obteniendo historial de estados para el vuelo {VueloId}", vueloId);
        var historial = await _unidadDeTrabajo.HistorialEstados.ObtenerPorVueloAsync(vueloId);
        var estados = await _unidadDeTrabajo.EstadosVuelo.ObtenerTodosAsync();
        var nombresPorId = estados.ToDictionary(e => e.Id, e => e.Nombre);

        return historial
            .OrderBy(h => h.FechaCambio)
            .Select(h => h.ADto(
                nombresPorId.GetValueOrDefault(h.EstadoAnteriorId, "Desconocido"),
                nombresPorId.GetValueOrDefault(h.EstadoNuevoId, "Desconocido")))
            .ToList();
    }

    public async Task RegistrarCambioDeEstadoAsync(int vueloId, int estadoAnteriorId, int estadoNuevoId)
    {
        _logger.LogInformation("Registrando cambio de estado para vuelo {VueloId}: {EstadoAnteriorId} -> {EstadoNuevoId}", vueloId, estadoAnteriorId, estadoNuevoId);
        var historial = new HistorialEstadoVuelo(vueloId, estadoAnteriorId, estadoNuevoId);
        await _unidadDeTrabajo.HistorialEstados.AgregarAsync(historial);
        await _unidadDeTrabajo.GuardarCambiosAsync();
    }
}
