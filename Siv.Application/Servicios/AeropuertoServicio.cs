using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Excepciones;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class AeropuertoServicio : IAeropuertoServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly IAuditoriaServicio _auditoriaServicio;
    private readonly ILogger<AeropuertoServicio> _logger;

    public AeropuertoServicio(IUnitOfWork unidadDeTrabajo, IAuditoriaServicio auditoriaServicio, ILogger<AeropuertoServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _auditoriaServicio = auditoriaServicio;
        _logger = logger;
    }

    public async Task<List<AeropuertoDto>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo todos los aeropuertos");
        var aeropuertos = await _unidadDeTrabajo.Aeropuertos.ObtenerTodosAsync();
        return aeropuertos
            .OrderBy(a => a.Nombre)
            .Select(a => a.ADto())
            .ToList();
    }

    public async Task<AeropuertoDto?> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Obteniendo aeropuerto con id {Id}", id);
        var aeropuerto = await _unidadDeTrabajo.Aeropuertos.ObtenerPorIdAsync(id);
        return aeropuerto?.ADto();
    }

    public async Task<AeropuertoDto> CrearAsync(CrearAeropuertoDto dto)
    {
        _logger.LogInformation("Iniciando creación de aeropuerto con código {Codigo}", dto.Codigo);
        var existeCodigo = await _unidadDeTrabajo.Aeropuertos.ExisteCodigoAsync(dto.Codigo);

        if (existeCodigo)
        {
            _logger.LogWarning("Fallo al crear aeropuerto: Ya existe un aeropuerto con el código '{Codigo}'.", dto.Codigo);
            throw new ExcepcionDeValidacion($"Ya existe un aeropuerto con el código '{dto.Codigo}'.");
        }

        var aeropuerto = new Aeropuerto(dto.Codigo, dto.Nombre, dto.Ciudad, dto.Pais);

        await _unidadDeTrabajo.Aeropuertos.AgregarAsync(aeropuerto);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Crear", "Aeropuertos", $"Se registró el aeropuerto {aeropuerto.Codigo} - {aeropuerto.Nombre}.");

        return aeropuerto.ADto();
    }

    public async Task ActualizarAsync(ActualizarAeropuertoDto dto)
    {
        _logger.LogInformation("Iniciando actualización de aeropuerto con id {Id}", dto.AeropuertoId);
        var aeropuerto = await _unidadDeTrabajo.Aeropuertos.ObtenerPorIdAsync(dto.AeropuertoId);

        if (aeropuerto == null)
        {
            _logger.LogError("Fallo al actualizar aeropuerto: No se encontró aeropuerto con id {Id}.", dto.AeropuertoId);
            throw new ExcepcionRecursoNoEncontrado($"No existe un aeropuerto con id {dto.AeropuertoId}.");
        }

        aeropuerto.ActualizarDatos(dto.Codigo, dto.Nombre, dto.Ciudad, dto.Pais);

        _unidadDeTrabajo.Aeropuertos.Actualizar(aeropuerto);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Actualizar", "Aeropuertos", $"Se actualizó el aeropuerto {aeropuerto.Id}.");
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogInformation("Iniciando eliminación de aeropuerto con id {Id}", id);
        var aeropuerto = await _unidadDeTrabajo.Aeropuertos.ObtenerPorIdAsync(id);

        if (aeropuerto == null)
        {
            _logger.LogWarning("No se encontró el aeropuerto con id {Id} para eliminar.", id);
            return;
        }

        var tieneVuelos = await _unidadDeTrabajo.Aeropuertos.TieneVuelosAsociadosAsync(id);

        if (tieneVuelos)
        {
            _logger.LogWarning("Fallo al eliminar aeropuerto {Id}: Tiene vuelos asociados.", id);
            throw new ExcepcionDeValidacion(
                "No se puede eliminar el aeropuerto porque tiene vuelos asociados (como origen o destino).");
        }

        _unidadDeTrabajo.Aeropuertos.Eliminar(aeropuerto);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Eliminar", "Aeropuertos", $"Se eliminó el aeropuerto {aeropuerto.Codigo} - {aeropuerto.Nombre}.");
    }
}
