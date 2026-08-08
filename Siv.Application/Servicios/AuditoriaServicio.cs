using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class AuditoriaServicio : IAuditoriaServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly ILogger<AuditoriaServicio> _logger;

    public AuditoriaServicio(IUnitOfWork unidadDeTrabajo, ILogger<AuditoriaServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _logger = logger;
    }

    public async Task RegistrarAsync(string accion, string tabla, string descripcion, string usuario = "Sistema", int? registroId = null, string? valorAnterior = null, string? valorNuevo = null)
    {
        _logger.LogInformation("Registrando auditoría para la tabla {Tabla} (Acción: {Accion})", tabla, accion);
        var auditoria = new Auditoria(accion, tabla, descripcion, usuario, registroId, valorAnterior, valorNuevo);
        await _unidadDeTrabajo.Auditorias.AgregarAsync(auditoria);
        await _unidadDeTrabajo.GuardarCambiosAsync();
    }

    public async Task<List<AuditoriaDto>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo todos los registros de auditoría");
        var registros = await _unidadDeTrabajo.Auditorias.ObtenerTodosAsync();
        return registros
            .OrderByDescending(a => a.Fecha)
            .Select(a => a.ADto())
            .ToList();
    }

    public async Task<List<AuditoriaDto>> ObtenerPorTablaAsync(string tabla)
    {
        _logger.LogInformation("Obteniendo registros de auditoría para la tabla {Tabla}", tabla);
        var registros = await _unidadDeTrabajo.Auditorias.ObtenerPorTablaAsync(tabla);
        return registros
            .OrderByDescending(a => a.Fecha)
            .Select(a => a.ADto())
            .ToList();
    }
}
