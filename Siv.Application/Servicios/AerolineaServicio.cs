using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Excepciones;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class AerolineaServicio : IAerolineaServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly IAuditoriaServicio _auditoriaServicio;
    private readonly ILogger<AerolineaServicio> _logger;

    public AerolineaServicio(IUnitOfWork unidadDeTrabajo, IAuditoriaServicio auditoriaServicio, ILogger<AerolineaServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _auditoriaServicio = auditoriaServicio;
        _logger = logger;
    }

    public async Task<List<AerolineaDto>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo todas las aerolíneas");
        var aerolineas = await _unidadDeTrabajo.Aerolineas.ObtenerTodosAsync();
        return aerolineas
            .OrderBy(a => a.Nombre)
            .Select(a => a.ADto())
            .ToList();
    }

    public async Task<AerolineaDto?> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Obteniendo aerolínea con id {Id}", id);
        var aerolinea = await _unidadDeTrabajo.Aerolineas.ObtenerPorIdAsync(id);
        return aerolinea?.ADto();
    }

    public async Task<AerolineaDto> CrearAsync(CrearAerolineaDto dto)
    {
        _logger.LogInformation("Iniciando creación de aerolínea con código {Codigo}", dto.Codigo);
        var existeCodigo = await _unidadDeTrabajo.Aerolineas.ExisteCodigoAsync(dto.Codigo);

        if (existeCodigo)
        {
            _logger.LogWarning("Fallo al crear aerolínea: Ya existe una aerolínea con el código '{Codigo}'.", dto.Codigo);
            throw new ExcepcionDeValidacion($"Ya existe una aerolínea con el código '{dto.Codigo}'.");
        }

        var aerolinea = new Aerolinea(dto.Codigo, dto.Nombre);

        await _unidadDeTrabajo.Aerolineas.AgregarAsync(aerolinea);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Crear", "Aerolineas", $"Se registró la aerolínea {aerolinea.Codigo} - {aerolinea.Nombre}.");

        return aerolinea.ADto();
    }

    public async Task ActualizarAsync(ActualizarAerolineaDto dto)
    {
        _logger.LogInformation("Iniciando actualización de aerolínea con id {Id}", dto.AerolineaId);
        var aerolinea = await _unidadDeTrabajo.Aerolineas.ObtenerPorIdAsync(dto.AerolineaId);

        if (aerolinea == null)
        {
            _logger.LogError("Fallo al actualizar aerolínea: No se encontró aerolínea con id {Id}.", dto.AerolineaId);
            throw new ExcepcionRecursoNoEncontrado($"No existe una aerolínea con id {dto.AerolineaId}.");
        }

        aerolinea.ActualizarDatos(dto.Codigo, dto.Nombre);

        _unidadDeTrabajo.Aerolineas.Actualizar(aerolinea);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Actualizar", "Aerolineas", $"Se actualizó la aerolínea {aerolinea.Id}.");
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogInformation("Iniciando eliminación de aerolínea con id {Id}", id);
        var aerolinea = await _unidadDeTrabajo.Aerolineas.ObtenerPorIdAsync(id);

        if (aerolinea == null)
        {
            _logger.LogWarning("No se encontró la aerolínea con id {Id} para eliminar.", id);
            return;
        }

        var tieneVuelos = await _unidadDeTrabajo.Aerolineas.TieneVuelosAsociadosAsync(id);

        if (tieneVuelos)
        {
            _logger.LogWarning("Fallo al eliminar aerolínea {Id}: Tiene vuelos asociados.", id);
            throw new ExcepcionDeValidacion(
                "No se puede eliminar la aerolínea porque tiene vuelos asociados.");
        }

        _unidadDeTrabajo.Aerolineas.Eliminar(aerolinea);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Eliminar", "Aerolineas", $"Se eliminó la aerolínea {aerolinea.Codigo} - {aerolinea.Nombre}.");
    }
}
