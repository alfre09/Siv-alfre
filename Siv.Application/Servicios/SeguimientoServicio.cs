using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Excepciones;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class SeguimientoServicio : ISeguimientoServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly IAuditoriaServicio _auditoriaServicio;
    private readonly ILogger<SeguimientoServicio> _logger;

    public SeguimientoServicio(IUnitOfWork unidadDeTrabajo, IAuditoriaServicio auditoriaServicio, ILogger<SeguimientoServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _auditoriaServicio = auditoriaServicio;
        _logger = logger;
    }

    public async Task<List<SeguimientoDto>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo todos los seguimientos");
        var seguimientos = await _unidadDeTrabajo.Seguimientos.ObtenerTodosAsync();
        return seguimientos.Select(s => s.ADto()).ToList();
    }

    public async Task<SeguimientoDto?> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Obteniendo seguimiento con id {Id}", id);
        var seguimiento = await _unidadDeTrabajo.Seguimientos.ObtenerPorIdAsync(id);
        return seguimiento?.ADto();
    }

    public async Task<SeguimientoDto?> ObtenerPorVueloYUsuarioAsync(int vueloId, string usuario)
    {
        _logger.LogInformation("Obteniendo seguimiento para vuelo {VueloId} y usuario {Usuario}", vueloId, usuario);
        var seguimiento = await _unidadDeTrabajo.Seguimientos.ObtenerPorVueloYUsuarioAsync(vueloId, usuario);
        return seguimiento?.ADto();
    }

    public async Task<List<SeguimientoDto>> ObtenerPorUsuarioAsync(string usuario)
    {
        _logger.LogInformation("Obteniendo todos los seguimientos para el usuario {Usuario}", usuario);
        var seguimientos = await _unidadDeTrabajo.Seguimientos.ObtenerTodosAsync();
        return seguimientos.Where(s => s.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase)).Select(s => s.ADto()).ToList();
    }

    public async Task<SeguimientoDto> CrearAsync(CrearSeguimientoDto dto)
    {
        _logger.LogInformation("Iniciando creación de seguimiento para el vuelo {VueloId}", dto.VueloId);
        var usuario = dto.Usuario?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(usuario))
        {
            _logger.LogWarning("Fallo al crear seguimiento: El usuario es obligatorio.");
            throw new ExcepcionDeValidacion("El usuario es obligatorio.");
        }

        if (dto.VueloId <= 0)
        {
            _logger.LogWarning("Fallo al crear seguimiento: El id del vuelo debe ser válido ({VueloId}).", dto.VueloId);
            throw new ExcepcionDeValidacion("El id del vuelo debe ser válido.");
        }

        var vuelo = await _unidadDeTrabajo.Vuelos.ObtenerPorIdAsync(dto.VueloId);

        if (vuelo == null)
        {
            _logger.LogError("Fallo al crear seguimiento: No existe un vuelo con id {VueloId}.", dto.VueloId);
            throw new ExcepcionRecursoNoEncontrado($"No existe un vuelo con id {dto.VueloId}.");
        }

        var yaExiste = await _unidadDeTrabajo.Seguimientos.ExisteSeguimientoAsync(dto.VueloId, usuario);

        if (yaExiste)
        {
            _logger.LogInformation(
                "El usuario {Usuario} ya seguía el vuelo {VueloId}; se devuelve el seguimiento existente.",
                usuario, dto.VueloId);

            var seguimientoExistente = await _unidadDeTrabajo.Seguimientos
                .ObtenerPorVueloYUsuarioAsync(dto.VueloId, usuario);

            if (seguimientoExistente is not null)
                return seguimientoExistente.ADto();
        }

        var seguimiento = new Seguimiento(usuario, dto.VueloId);

        await _unidadDeTrabajo.Seguimientos.AgregarAsync(seguimiento);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Crear", "Seguimientos", $"El usuario {usuario} inició seguimiento del vuelo {dto.VueloId}.");

        return seguimiento.ADto();
    }

    public async Task ActualizarAsync(ActualizarSeguimientoDto dto)
    {
        _logger.LogInformation("Iniciando actualización de seguimiento con id {Id}", dto.SeguimientoId);
        var seguimiento = await _unidadDeTrabajo.Seguimientos.ObtenerPorIdAsync(dto.SeguimientoId);

        if (seguimiento == null)
        {
            _logger.LogError("Fallo al actualizar seguimiento: No existe seguimiento con id {Id}.", dto.SeguimientoId);
            throw new ExcepcionRecursoNoEncontrado(
                $"No existe un seguimiento con id {dto.SeguimientoId}.");
        }

        var usuario = dto.Usuario?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(usuario))
        {
            _logger.LogWarning("Fallo al actualizar seguimiento: El usuario es obligatorio.");
            throw new ExcepcionDeValidacion("El usuario es obligatorio.");
        }

        if (dto.VueloId <= 0)
        {
            _logger.LogWarning("Fallo al actualizar seguimiento: El id del vuelo debe ser válido ({VueloId}).", dto.VueloId);
            throw new ExcepcionDeValidacion("El id del vuelo debe ser válido.");
        }

        var vuelo = await _unidadDeTrabajo.Vuelos.ObtenerPorIdAsync(dto.VueloId);

        if (vuelo == null)
        {
            _logger.LogError("Fallo al actualizar seguimiento: No existe un vuelo con id {VueloId}.", dto.VueloId);
            throw new ExcepcionRecursoNoEncontrado($"No existe un vuelo con id {dto.VueloId}.");
        }

        var duplicado = await _unidadDeTrabajo.Seguimientos.ExisteSeguimientoAsync(dto.VueloId, usuario);

        if (duplicado && !(seguimiento.VueloId == dto.VueloId &&
                           seguimiento.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Fallo al actualizar seguimiento: El usuario {Usuario} ya está siguiendo el vuelo {VueloId}.", usuario, dto.VueloId);
            throw new ExcepcionDeValidacion("Este usuario ya está siguiendo este vuelo.");
        }

        seguimiento.Reasignar(usuario, dto.VueloId);

        _unidadDeTrabajo.Seguimientos.Actualizar(seguimiento);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Actualizar", "Seguimientos", $"Se actualizó el seguimiento {seguimiento.Id}.");
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogInformation("Iniciando eliminación de seguimiento con id {Id}", id);
        var seguimiento = await _unidadDeTrabajo.Seguimientos.ObtenerPorIdAsync(id);

        if (seguimiento == null)
        {
            _logger.LogWarning("No se encontró el seguimiento con id {Id} para eliminar.", id);
            return;
        }

        _unidadDeTrabajo.Seguimientos.Eliminar(seguimiento);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Eliminar", "Seguimientos",
            $"El usuario {seguimiento.Usuario} dejó de seguir el vuelo {seguimiento.VueloId}.");
    }
}
