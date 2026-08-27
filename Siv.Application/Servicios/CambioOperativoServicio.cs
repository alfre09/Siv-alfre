using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Excepciones;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Enums;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class CambioOperativoServicio : ICambioOperativoServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly INotificacionServicio _notificacionServicio;
    private readonly IAuditoriaServicio _auditoriaServicio;
    private readonly ILogger<CambioOperativoServicio> _logger;

    public CambioOperativoServicio(
        IUnitOfWork unidadDeTrabajo,
        INotificacionServicio notificacionServicio,
        IAuditoriaServicio auditoriaServicio,
        ILogger<CambioOperativoServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _notificacionServicio = notificacionServicio;
        _auditoriaServicio = auditoriaServicio;
        _logger = logger;
    }

    public async Task<List<CambioOperativoDto>> ObtenerTodosAsync()
    {
        _logger.LogInformation("Obteniendo todos los cambios operativos");
        var cambios = await _unidadDeTrabajo.CambiosOperativos.ObtenerTodosAsync();
        return cambios
            .OrderByDescending(c => c.FechaCambio)
            .Select(c => c.ADto())
            .ToList();
    }

    public async Task<List<CambioOperativoDto>> ObtenerPorVueloAsync(int vueloId, string? rolUsuario = null)
    {
        _logger.LogInformation("Obteniendo cambios operativos para el vuelo {VueloId}", vueloId);
        var vuelo = await _unidadDeTrabajo.Vuelos.ObtenerConDetalleAsync(vueloId);

        if (vuelo is null || !PoliticaVisibilidadVuelo.PuedeConsultar(vuelo.NivelVisibilidad, rolUsuario))
            return new List<CambioOperativoDto>();

        var cambios = await _unidadDeTrabajo.CambiosOperativos.ObtenerPorVueloAsync(vueloId);
        return cambios
            .OrderByDescending(c => c.FechaCambio)
            .Select(c => c.ADto())
            .ToList();
    }

    public async Task<CambioOperativoDto> RegistrarRetrasoOAdelantoAsync(RegistrarRetrasoOAdelantoDto dto)
    {
        _logger.LogInformation("Iniciando registro de retraso o adelanto para vuelo {VueloId}", dto.VueloId);
        var vuelo = await ObtenerVueloConEstadoAsync(dto.VueloId);

        if (dto.NuevoHorario == vuelo.HorarioProgramado)
        {
            _logger.LogWarning("Fallo de validación: El nuevo horario es igual al actual para vuelo {VueloId}.", dto.VueloId);
            throw new ExcepcionDeValidacion("El nuevo horario debe ser diferente al horario actual.");
        }

        if (dto.EsAdelanto && dto.NuevoHorario >= vuelo.HorarioProgramado)
        {
            _logger.LogWarning("Fallo de validación: El nuevo horario no es anterior al actual para adelanto de vuelo {VueloId}.", dto.VueloId);
            throw new ExcepcionDeValidacion("El nuevo horario debe ser anterior al actual para registrar un adelanto.");
        }

        if (!dto.EsAdelanto && dto.NuevoHorario <= vuelo.HorarioProgramado)
        {
            _logger.LogWarning("Fallo de validación: El nuevo horario no es posterior al actual para retraso de vuelo {VueloId}.", dto.VueloId);
            throw new ExcepcionDeValidacion("El nuevo horario debe ser posterior al actual para registrar un retraso.");
        }

        var (valorAnterior, valorNuevo) = vuelo.AplicarCambioDeHorario(dto.NuevoHorario);
        var tipo = dto.EsAdelanto ? TipoCambioOperativo.Adelanto : TipoCambioOperativo.Retraso;

        return await FinalizarRegistroAsync(vuelo, tipo, dto.Causa, valorAnterior, valorNuevo);
    }

    public async Task<CambioOperativoDto> RegistrarCambioPuertaAsync(RegistrarCambioPuertaDto dto)
    {
        _logger.LogInformation("Iniciando registro de cambio de puerta para vuelo {VueloId}", dto.VueloId);
        var vuelo = await ObtenerVueloConEstadoAsync(dto.VueloId);
        
        var nombrePuerta = dto.NuevaPuerta?.Trim().ToUpperInvariant() ?? string.Empty;
        var puertas = await _unidadDeTrabajo.Puertas.ObtenerTodosAsync();
        var puertaExiste = puertas.FirstOrDefault(p =>
            p.AeropuertoId == vuelo.AeropuertoOrigenId &&
            p.Nombre.Equals(nombrePuerta, StringComparison.OrdinalIgnoreCase));

        if (puertaExiste is null)
        {
            _logger.LogWarning("Fallo de validación: La puerta {NuevaPuerta} no existe en el sistema.", dto.NuevaPuerta);
            throw new ExcepcionDeValidacion($"La puerta '{nombrePuerta}' no está disponible para el aeropuerto del vuelo.");
        }

        var vuelos = await _unidadDeTrabajo.Vuelos.ObtenerTodosConDetalleAsync();
        if (vuelos.Any(v =>
            v.Id != vuelo.Id &&
            v.AeropuertoOrigenId == vuelo.AeropuertoOrigenId &&
            string.Equals(v.Puerta, puertaExiste.Nombre, StringComparison.OrdinalIgnoreCase) &&
            v.EstadoVuelo is not null &&
            !v.EstadoVuelo.EsFinal() &&
            Math.Abs((v.HorarioProgramado - vuelo.HorarioProgramado).TotalMinutes) < 120))
        {
            throw new ExcepcionDeValidacion(
                $"La puerta '{puertaExiste.Nombre}' no está disponible para ese horario.");
        }

        var (valorAnterior, valorNuevo) = vuelo.AplicarCambioDePuerta(puertaExiste.Nombre);

        if (string.Equals(valorAnterior, valorNuevo, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Fallo de validación: La nueva puerta es igual a la actual para vuelo {VueloId}.", dto.VueloId);
            throw new ExcepcionDeValidacion("La nueva puerta debe ser diferente a la puerta actual.");
        }

        return await FinalizarRegistroAsync(
            vuelo, TipoCambioOperativo.CambioPuerta, dto.Causa, valorAnterior, valorNuevo);
    }

    public async Task<CambioOperativoDto> RegistrarCambioEstadoAsync(RegistrarCambioEstadoDto dto)
    {
        _logger.LogInformation("Iniciando registro de cambio de estado para vuelo {VueloId}", dto.VueloId);
        var vuelo = await ObtenerVueloConEstadoAsync(dto.VueloId);

        var estadoNuevo = await _unidadDeTrabajo.EstadosVuelo.ObtenerPorIdAsync(dto.NuevoEstadoVueloId);

        if (estadoNuevo == null)
        {
            _logger.LogError("Fallo al registrar cambio de estado: No existe estado {EstadoId}.", dto.NuevoEstadoVueloId);
            throw new ExcepcionRecursoNoEncontrado($"No existe el estado con id {dto.NuevoEstadoVueloId}.");
        }

        var (valorAnterior, valorNuevo, estadoAnteriorId) = vuelo.AplicarCambioDeEstado(estadoNuevo);

        await _unidadDeTrabajo.HistorialEstados.AgregarAsync(
            new HistorialEstadoVuelo(vuelo.Id, estadoAnteriorId, estadoNuevo.Id));

        return await FinalizarRegistroAsync(
            vuelo, TipoCambioOperativo.CambioEstado, dto.Causa, valorAnterior, valorNuevo);
    }

    public async Task<CambioOperativoDto> RegistrarCancelacionAsync(RegistrarCancelacionDto dto)
    {
        _logger.LogInformation("Iniciando registro de cancelación para vuelo {VueloId}", dto.VueloId);
        var vuelo = await ObtenerVueloConEstadoAsync(dto.VueloId);

        var estadoCancelado = await _unidadDeTrabajo.EstadosVuelo.ObtenerPorNombreAsync(EstadoVuelo.Cancelado);

        if (estadoCancelado == null)
        {
            _logger.LogError("Error crítico: No existe el estado '{Cancelado}' en el catálogo.", EstadoVuelo.Cancelado);
            throw new ExcepcionRecursoNoEncontrado(
                $"No existe el estado '{EstadoVuelo.Cancelado}' en el catálogo. Verifica el seed de datos.");
        }

        var (valorAnterior, valorNuevo, estadoAnteriorId) = vuelo.AplicarCambioDeEstado(estadoCancelado);

        await _unidadDeTrabajo.HistorialEstados.AgregarAsync(
            new HistorialEstadoVuelo(vuelo.Id, estadoAnteriorId, estadoCancelado.Id));

        return await FinalizarRegistroAsync(
            vuelo, TipoCambioOperativo.Cancelacion, dto.Causa, valorAnterior, valorNuevo);
    }

    private async Task<Vuelo> ObtenerVueloConEstadoAsync(int vueloId)
    {
        if (vueloId <= 0)
        {
            _logger.LogWarning("El id del vuelo proporcionado es inválido ({VueloId}).", vueloId);
            throw new ExcepcionDeValidacion("El id del vuelo debe ser válido.");
        }

        var vuelo = await _unidadDeTrabajo.Vuelos.ObtenerConDetalleAsync(vueloId);

        if (vuelo == null)
        {
            _logger.LogError("No se encontró el vuelo con id {VueloId}.", vueloId);
            throw new ExcepcionRecursoNoEncontrado($"No existe un vuelo con id {vueloId}.");
        }

        return vuelo;
    }

    private async Task<CambioOperativoDto> FinalizarRegistroAsync(
        Vuelo vuelo,
        TipoCambioOperativo tipo,
        string causa,
        string valorAnterior,
        string valorNuevo)
    {
        _logger.LogInformation("Finalizando registro de cambio operativo de tipo {Tipo} para vuelo {VueloId}", tipo, vuelo.Id);
        CambioOperativoDto? resultado = null;

        await _unidadDeTrabajo.EjecutarEnTransaccionAsync(async () =>
        {
            _unidadDeTrabajo.Vuelos.Actualizar(vuelo);

            var cambioOperativo = new CambioOperativo(vuelo.Id, tipo, causa, valorAnterior, valorNuevo);
            await _unidadDeTrabajo.CambiosOperativos.AgregarAsync(cambioOperativo);

            await _unidadDeTrabajo.GuardarCambiosAsync();

            resultado = await RegistrarCambioDesdeEdicionSinTransaccionAsync(
                vuelo.Id, vuelo.NumeroVuelo, cambioOperativo, valorAnterior, valorNuevo);
        });

        return resultado!;
    }

    public async Task<CambioOperativoDto> RegistrarCambioDesdeEdicionAsync(
        int vueloId,
        string numeroVuelo,
        TipoCambioOperativo tipo,
        string causa,
        string valorAnterior,
        string valorNuevo)
    {
        _logger.LogInformation("Registrando cambio desde edición para vuelo {VueloId}, tipo {Tipo}", vueloId, tipo);
        CambioOperativoDto? resultado = null;

        await _unidadDeTrabajo.EjecutarEnTransaccionAsync(async () =>
        {
            var cambioOperativo = new CambioOperativo(vueloId, tipo, causa, valorAnterior, valorNuevo);
            await _unidadDeTrabajo.CambiosOperativos.AgregarAsync(cambioOperativo);
            await _unidadDeTrabajo.GuardarCambiosAsync();

            resultado = await RegistrarCambioDesdeEdicionSinTransaccionAsync(
                vueloId, numeroVuelo, cambioOperativo, valorAnterior, valorNuevo);
        });

        return resultado!;
    }

    private async Task<CambioOperativoDto> RegistrarCambioDesdeEdicionSinTransaccionAsync(
        int vueloId,
        string numeroVuelo,
        CambioOperativo cambioOperativo,
        string valorAnterior,
        string valorNuevo)
    {
        await _auditoriaServicio.RegistrarAsync(
            "Registrar", "CambiosOperativos",
            $"Vuelo {numeroVuelo} (id {vueloId}): {cambioOperativo.TipoCambio.ANombreLegible()} de '{valorAnterior}' a '{valorNuevo}'. Causa: {cambioOperativo.Causa}");

        var mensaje = $"El vuelo {numeroVuelo} tuvo un cambio ({cambioOperativo.TipoCambio.ANombreLegible()}): " +
                      $"{valorAnterior} → {valorNuevo}. Causa: {cambioOperativo.Causa}";

        await _notificacionServicio.GenerarNotificacionesPorCambioAsync(
            vueloId, cambioOperativo.Id, mensaje);

        return cambioOperativo.ADto();
    }
}
