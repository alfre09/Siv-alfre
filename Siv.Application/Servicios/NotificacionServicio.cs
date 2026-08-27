using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class NotificacionServicio : INotificacionServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly IAuditoriaServicio _auditoriaServicio;
    private readonly INotificadorTiempoReal _notificadorTiempoReal;
    private readonly IUsuarioServicio _usuarioServicio;
    private readonly IEmailServicio _emailServicio;
    private readonly ILogger<NotificacionServicio> _logger;

    public NotificacionServicio(
        IUnitOfWork unidadDeTrabajo,
        IAuditoriaServicio auditoriaServicio,
        INotificadorTiempoReal notificadorTiempoReal,
        IUsuarioServicio usuarioServicio,
        IEmailServicio emailServicio,
        ILogger<NotificacionServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _auditoriaServicio = auditoriaServicio;
        _notificadorTiempoReal = notificadorTiempoReal;
        _usuarioServicio = usuarioServicio;
        _emailServicio = emailServicio;
        _logger = logger;
    }

    public async Task<List<NotificacionDto>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo todas las notificaciones");
        var notificaciones = await _unidadDeTrabajo.Notificaciones.ObtenerTodosAsync();
        return notificaciones
            .OrderByDescending(n => n.FechaEnvio)
            .Select(n => n.ADto())
            .ToList();
    }

    public async Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(string usuario)
    {
        _logger.LogInformation("Obteniendo notificaciones para el usuario {Usuario}", usuario);
        if (string.IsNullOrWhiteSpace(usuario))
            return new List<NotificacionDto>();

        var notificaciones = await _unidadDeTrabajo.Notificaciones.ObtenerPorUsuarioAsync(usuario.Trim());
        return notificaciones
            .OrderByDescending(n => n.FechaEnvio)
            .Select(n => n.ADto())
            .ToList();
    }

    public async Task<List<NotificacionDto>> ObtenerPorVueloAsync(int vueloId)
    {
        _logger.LogInformation("Obteniendo notificaciones para el vuelo {VueloId}", vueloId);
        var notificaciones = await _unidadDeTrabajo.Notificaciones.ObtenerPorVueloAsync(vueloId);
        return notificaciones
            .OrderByDescending(n => n.FechaEnvio)
            .Select(n => n.ADto())
            .ToList();
    }

    public async Task MarcarComoLeidaAsync(int notificacionId, string usuario)
    {
        _logger.LogInformation("Marcando notificación {Id} como leída", notificacionId);
        var notificacion = await _unidadDeTrabajo.Notificaciones.ObtenerPorIdAsync(notificacionId);

        if (notificacion == null ||
            !notificacion.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("No se encontró la notificación con id {Id} para marcar como leída.", notificacionId);
            return;
        }

        notificacion.MarcarComoLeida();

        _unidadDeTrabajo.Notificaciones.Actualizar(notificacion);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Actualizar", "Notificaciones", $"Notificación {notificacion.Id} marcada como leída.");
    }

    public async Task GenerarNotificacionesPorCambioAsync(int vueloId, int cambioOperativoId, string mensaje)
    {
        _logger.LogInformation("Iniciando generación de notificaciones para vuelo {VueloId}, cambio {CambioId}", vueloId, cambioOperativoId);
        if (cambioOperativoId <= 0)
        {
            _logger.LogWarning("Fallo al generar notificaciones: El cambio operativo debe ser válido ({CambioId}).", cambioOperativoId);
            throw new ArgumentException("El cambio operativo debe ser válido.", nameof(cambioOperativoId));
        }

        var interesados = await _unidadDeTrabajo.Seguimientos.ObtenerUsuariosInteresadosAsync(vueloId);

        if (interesados.Count == 0)
            return;

        var notificaciones = interesados
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(usuario => new Notificacion(vueloId, cambioOperativoId, usuario, mensaje))
            .ToList();

        await _unidadDeTrabajo.Notificaciones.AgregarRangoAsync(notificaciones);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        // Enviar y auditar el resultado individual de cada entrega en tiempo real.
        foreach (var notificacion in notificaciones)
        {
            var enviado = false;
            string resultado;

            try
            {
                enviado = await _notificadorTiempoReal.EnviarNotificacionAsync(
                    notificacion.Usuario,
                    notificacion.Mensaje);
                resultado = enviado ? "Enviado" : "Fallido";
            }
            catch (Exception ex)
            {
                resultado = "Fallido";
                _logger.LogError(
                    ex,
                    "No se pudo entregar la notificación {NotificacionId} al usuario {Usuario}.",
                    notificacion.Id,
                    notificacion.Usuario);
            }

            await _auditoriaServicio.RegistrarAsync(
                enviado ? "Enviar" : "ErrorEnvio",
                "Notificaciones",
                $"Entrega de la notificación {notificacion.Id} al usuario {notificacion.Usuario} " +
                $"para el cambio operativo {cambioOperativoId}: {resultado}.",
                registroId: notificacion.Id,
                valorAnterior: "Pendiente",
                valorNuevo: resultado);

            try
            {
                var correo = await _usuarioServicio.ObtenerCorreoAsync(notificacion.Usuario);
                if (string.IsNullOrWhiteSpace(correo))
                {
                    _logger.LogWarning("El usuario {Usuario} no tiene correo registrado; se conserva la notificación interna.", notificacion.Usuario);
                    continue;
                }

                await _emailServicio.EnviarAsync(correo, "Actualización operativa de tu vuelo", notificacion.Mensaje);
            }
            catch (Exception excepcion)
            {
                // Un fallo de correo no debe deshacer el cambio operativo ni la notificación guardada.
                _logger.LogError(excepcion, "No se pudo enviar por correo la notificación al usuario {Usuario}.", notificacion.Usuario);
            }
        }

        await _auditoriaServicio.RegistrarAsync(
            "Generar", "Notificaciones",
            $"Se generaron {notificaciones.Count} notificación(es) para el cambio operativo {cambioOperativoId} del vuelo {vueloId}.");
    }
}
