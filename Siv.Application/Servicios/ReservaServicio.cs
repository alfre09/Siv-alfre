using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Excepciones;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Enums;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class ReservaServicio : IReservaServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly IAuditoriaServicio _auditoriaServicio;
    private readonly ILogger<ReservaServicio> _logger;

    public ReservaServicio(
        IUnitOfWork unidadDeTrabajo,
        IAuditoriaServicio auditoriaServicio,
        ILogger<ReservaServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _auditoriaServicio = auditoriaServicio;
        _logger = logger;
    }

    public async Task<ReservaDto> CrearAsync(int vueloId, string usuario)
    {
        var usuarioNormalizado = usuario?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(usuarioNormalizado))
            throw new ExcepcionDeValidacion("El usuario es obligatorio para reservar.");
        if (vueloId <= 0)
            throw new ExcepcionDeValidacion("Debes seleccionar un vuelo válido.");

        var vuelo = await _unidadDeTrabajo.Vuelos.ObtenerConDetalleAsync(vueloId);
        if (vuelo is null)
            throw new ExcepcionRecursoNoEncontrado($"No existe un vuelo con id {vueloId}.");

        if (vuelo.NivelVisibilidad != NivelVisibilidad.Publico)
            throw new ExcepcionDeValidacion("Solo se pueden reservar vuelos públicos.");

        if (vuelo.EstadoVuelo is null || vuelo.EstadoVuelo.EsFinal())
            throw new ExcepcionDeValidacion("Este vuelo ya no está disponible para reservar.");

        var reservaExistente = await _unidadDeTrabajo.Reservas.ObtenerActivaAsync(usuarioNormalizado, vueloId);
        if (reservaExistente is not null)
            throw new ExcepcionDeValidacion("Ya tienes una reserva activa para este vuelo.");

        var reserva = new Reserva(usuarioNormalizado, vueloId);
        await _unidadDeTrabajo.Reservas.AgregarAsync(reserva);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Crear", "Reservas", $"El usuario {usuarioNormalizado} reservó el vuelo {vuelo.NumeroVuelo}.");

        reserva = await _unidadDeTrabajo.Reservas.ObtenerPorIdAsync(reserva.Id) ?? reserva;
        reserva.AsociarVuelo(vuelo);
        return reserva.ADto();
    }

    public async Task<List<ReservaDto>> ObtenerPorUsuarioAsync(string usuario)
    {
        var usuarioNormalizado = usuario?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(usuarioNormalizado))
            throw new ExcepcionDeValidacion("El usuario es obligatorio.");

        var reservas = await _unidadDeTrabajo.Reservas.ObtenerPorUsuarioAsync(usuarioNormalizado);
        return reservas.Select(r => r.ADto()).ToList();
    }

    public async Task CancelarAsync(int reservaId, string usuario)
    {
        var usuarioNormalizado = usuario?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(usuarioNormalizado))
            throw new ExcepcionDeValidacion("El usuario es obligatorio.");

        var reserva = await _unidadDeTrabajo.Reservas.ObtenerPorIdAsync(reservaId);
        if (reserva is null || !reserva.Usuario.Equals(usuarioNormalizado, StringComparison.OrdinalIgnoreCase))
            throw new ExcepcionRecursoNoEncontrado("No existe una reserva perteneciente a tu usuario con ese id.");

        if (reserva.Estado == EstadoReserva.Cancelada)
            return;

        reserva.Cancelar();
        _unidadDeTrabajo.Reservas.Actualizar(reserva);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Cancelar", "Reservas", $"El usuario {usuarioNormalizado} canceló la reserva {reserva.Id}.");
    }
}
