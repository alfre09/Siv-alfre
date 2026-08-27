using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Excepciones;
using Siv.Application.Interfaces;
using Siv.Application.Mapeadores;
using Siv.Domain.Entidades;
using Siv.Domain.Enums;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class VueloServicio : IVueloServicio
{
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly IAuditoriaServicio _auditoriaServicio;
    private readonly ICambioOperativoServicio _cambioOperativoServicio;
    private readonly ILogger<VueloServicio> _logger;

    public VueloServicio(
        IUnitOfWork unidadDeTrabajo,
        IAuditoriaServicio auditoriaServicio,
        ICambioOperativoServicio cambioOperativoServicio,
        ILogger<VueloServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _auditoriaServicio = auditoriaServicio;
        _cambioOperativoServicio = cambioOperativoServicio;
        _logger = logger;
    }

    public async Task<List<VueloDto>> ObtenerTodosAsync(string? rolUsuario = null)
    {
        _logger.LogInformation("Obteniendo todos los vuelos para rol {Rol}", rolUsuario ?? "Anónimo");
        var vuelos = await _unidadDeTrabajo.Vuelos.ObtenerTodosConDetalleAsync();

        // Filtrar por nivel de visibilidad según el rol del usuario
        vuelos = vuelos
            .Where(v => PoliticaVisibilidadVuelo.PuedeConsultar(v.NivelVisibilidad, rolUsuario))
            .ToList();

        return vuelos.Select(v => v.ADto()).ToList();
    }

    public async Task<List<VueloDto>> ObtenerDisponiblesAsync(int? origenId = null, int? destinoId = null, DateTime? fecha = null)
    {
        var vuelos = await _unidadDeTrabajo.Vuelos.ObtenerTodosConDetalleAsync();

        var disponibles = vuelos.Where(v =>
                v.NivelVisibilidad == NivelVisibilidad.Publico &&
                v.EstadoVuelo is not null &&
                !v.EstadoVuelo.EsFinal())
            .Where(v => !origenId.HasValue || v.AeropuertoOrigenId == origenId.Value)
            .Where(v => !destinoId.HasValue || v.AeropuertoDestinoId == destinoId.Value)
            .Where(v => !fecha.HasValue || v.HorarioProgramado.Date == fecha.Value.Date)
            .OrderBy(v => v.HorarioProgramado)
            .Select(v => v.ADto())
            .ToList();

        return disponibles;
    }

    public async Task<VueloDto?> ObtenerPorIdAsync(int id, string? rolUsuario = null)
    {
        _logger.LogInformation("Obteniendo vuelo con id {Id}", id);
        var vuelo = await _unidadDeTrabajo.Vuelos.ObtenerConDetalleAsync(id);

        if (vuelo is null || !PoliticaVisibilidadVuelo.PuedeConsultar(vuelo.NivelVisibilidad, rolUsuario))
            return null;

        var rolAuditoria = string.IsNullOrWhiteSpace(rolUsuario) ? "Usuario anónimo" : $"Rol: {rolUsuario}";
        await _auditoriaServicio.RegistrarAsync(
            "Consultar", 
            "Vuelos", 
            $"Se consultó el detalle del vuelo {vuelo.NumeroVuelo} (id {id}). {rolAuditoria}");

        return vuelo?.ADto();
    }

    public async Task<VueloDto> CrearAsync(CrearVueloDto dto)
    {
        _logger.LogInformation("Iniciando creación de vuelo con número {Numero}", dto.NumeroVuelo);

        if (dto.HorarioProgramado <= DateTime.Now)
        {
            throw new ExcepcionDeValidacion("El horario programado del vuelo debe ser en el futuro.");
        }

        if (dto.AeropuertoOrigenId == dto.AeropuertoDestinoId)
        {
            throw new ExcepcionDeValidacion("El aeropuerto de origen y el de destino deben ser diferentes.");
        }

        await ValidarReferenciasAsync(dto.AerolineaId, dto.AeropuertoOrigenId, dto.AeropuertoDestinoId);
        dto.Puerta = await ValidarPuertaDisponibleAsync(
            dto.Puerta, dto.AeropuertoOrigenId, dto.HorarioProgramado);

        var numeroVuelo = dto.NumeroVuelo?.Trim() ?? string.Empty;

        if (await _unidadDeTrabajo.Vuelos.ExisteNumeroVueloAsync(numeroVuelo))
        {
            _logger.LogWarning("Fallo al crear vuelo: Ya existe un vuelo con el número '{Numero}'.", numeroVuelo);
            throw new ExcepcionDeValidacion($"Ya existe un vuelo registrado con el número '{numeroVuelo}'.");
        }

        var estadoInicial = await _unidadDeTrabajo.EstadosVuelo.ObtenerPorNombreAsync(EstadoVuelo.Programado);

        if (estadoInicial == null)
        {
            _logger.LogError("Error crítico: No existe el estado '{Programado}' en el catálogo.", EstadoVuelo.Programado);
            throw new ExcepcionRecursoNoEncontrado(
                $"No existe el estado '{EstadoVuelo.Programado}' en el catálogo. Verifica el seed de datos.");
        }

        var vuelo = new Vuelo(
            numeroVuelo,
            dto.AerolineaId,
            dto.AeropuertoOrigenId,
            dto.AeropuertoDestinoId,
            dto.HorarioProgramado,
            estadoInicial.Id,
            dto.Puerta,
            dto.NivelVisibilidad);

        await _unidadDeTrabajo.Vuelos.AgregarAsync(vuelo);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Crear", "Vuelos", $"Se registró el vuelo {vuelo.NumeroVuelo} (id {vuelo.Id}).");

        var vueloCreado = await _unidadDeTrabajo.Vuelos.ObtenerConDetalleAsync(vuelo.Id);
        return vueloCreado!.ADto();
    }

    public async Task ActualizarAsync(ActualizarVueloDto dto, string? rolUsuario = null)
    {
        _logger.LogInformation("Iniciando actualización de vuelo con id {Id}", dto.VueloId);
        var vuelo = await _unidadDeTrabajo.Vuelos.ObtenerConDetalleAsync(dto.VueloId);

        if (vuelo == null)
        {
            _logger.LogError("Fallo al actualizar vuelo: No se encontró vuelo con id {Id}.", dto.VueloId);
            throw new ExcepcionRecursoNoEncontrado($"No existe un vuelo con id {dto.VueloId}.");
        }

        if (string.Equals(rolUsuario, "Operador", StringComparison.OrdinalIgnoreCase))
            dto.NivelVisibilidad = vuelo.NivelVisibilidad;

        if (dto.HorarioProgramado <= DateTime.Now)
        {
            throw new ExcepcionDeValidacion("El horario programado del vuelo debe ser en el futuro.");
        }

        if (dto.AeropuertoOrigenId == dto.AeropuertoDestinoId)
        {
            throw new ExcepcionDeValidacion("El aeropuerto de origen y el de destino deben ser diferentes.");
        }

        await ValidarReferenciasAsync(dto.AerolineaId, dto.AeropuertoOrigenId, dto.AeropuertoDestinoId);
        dto.Puerta = await ValidarPuertaDisponibleAsync(
            dto.Puerta, dto.AeropuertoOrigenId, dto.HorarioProgramado, dto.VueloId);

        var numeroVuelo = dto.NumeroVuelo?.Trim() ?? string.Empty;

        if (await _unidadDeTrabajo.Vuelos.ExisteNumeroVueloAsync(numeroVuelo, dto.VueloId))
        {
            _logger.LogWarning("Fallo al actualizar vuelo: Ya existe otro vuelo con el número '{Numero}'.", numeroVuelo);
            throw new ExcepcionDeValidacion($"Ya existe otro vuelo registrado con el número '{numeroVuelo}'.");
        }

        var horarioAnterior = vuelo.HorarioProgramado;
        var puertaAnterior = vuelo.Puerta;
        var aerolineaAnteriorId = vuelo.AerolineaId;
        var aerolineaAnteriorNombre = vuelo.Aerolinea?.Nombre ?? "(desconocida)";
        
        var origenAnteriorId = vuelo.AeropuertoOrigenId;
        var destinoAnteriorId = vuelo.AeropuertoDestinoId;
        var rutaAnterior = $"{vuelo.AeropuertoOrigen?.Codigo ?? "N/A"} - {vuelo.AeropuertoDestino?.Codigo ?? "N/A"}";

        var cambioHorario = Math.Abs((horarioAnterior - dto.HorarioProgramado).TotalMinutes) >= 1;
        var cambioPuerta = !string.Equals(puertaAnterior, dto.Puerta, StringComparison.OrdinalIgnoreCase);
        var cambioAerolinea = aerolineaAnteriorId != dto.AerolineaId;
        var cambioRuta = origenAnteriorId != dto.AeropuertoOrigenId || destinoAnteriorId != dto.AeropuertoDestinoId;

        if (cambioHorario || cambioPuerta || cambioAerolinea || cambioRuta)
        {
            vuelo.ValidarQueAdmiteCambioOperativo();

            if (string.IsNullOrWhiteSpace(dto.Causa))
            {
                _logger.LogWarning("Fallo al actualizar vuelo {Id}: No se indicó causa para el cambio operativo.", dto.VueloId);
                throw new ExcepcionDeValidacion(
                    "Debes indicar la causa cuando modificas la programación (horario, puerta, aerolínea o ruta).");
            }
        }

        var aerolineaNuevaNombre = aerolineaAnteriorNombre;
        if (cambioAerolinea)
        {
            var aerolinea = await _unidadDeTrabajo.Aerolineas.ObtenerPorIdAsync(dto.AerolineaId);
            aerolineaNuevaNombre = aerolinea?.Nombre ?? "(desconocida)";
        }

        var rutaNueva = rutaAnterior;
        if (cambioRuta)
        {
            var origenNuevo = await _unidadDeTrabajo.Aeropuertos.ObtenerPorIdAsync(dto.AeropuertoOrigenId);
            var destinoNuevo = await _unidadDeTrabajo.Aeropuertos.ObtenerPorIdAsync(dto.AeropuertoDestinoId);
            rutaNueva = $"{origenNuevo?.Codigo ?? "N/A"} - {destinoNuevo?.Codigo ?? "N/A"}";
        }

        await _unidadDeTrabajo.EjecutarEnTransaccionAsync(async () =>
        {
            vuelo.ActualizarProgramacion(
                numeroVuelo,
                dto.AerolineaId,
                dto.AeropuertoOrigenId,
                dto.AeropuertoDestinoId,
                dto.HorarioProgramado,
                dto.Puerta,
                dto.NivelVisibilidad);

            _unidadDeTrabajo.Vuelos.Actualizar(vuelo);
            await _unidadDeTrabajo.GuardarCambiosAsync();

            await _auditoriaServicio.RegistrarAsync(
                "Actualizar", "Vuelos", $"Se actualizó la programación del vuelo {vuelo.Id}.");

            await RegistrarCambiosOperativosSiAplicaAsync(
                vuelo, dto.Causa!, horarioAnterior, dto.HorarioProgramado, puertaAnterior, dto.Puerta, 
                aerolineaAnteriorId, dto.AerolineaId, aerolineaAnteriorNombre, aerolineaNuevaNombre,
                cambioRuta, rutaAnterior, rutaNueva);
        });
    }

    private async Task RegistrarCambiosOperativosSiAplicaAsync(
        Vuelo vuelo,
        string causa,
        DateTime horarioAnterior,
        DateTime horarioNuevo,
        string? puertaAnterior,
        string? puertaNueva,
        int aerolineaAnteriorId,
        int aerolineaNuevaId,
        string aerolineaAnteriorNombre,
        string aerolineaNuevaNombre,
        bool cambioRuta,
        string rutaAnterior,
        string rutaNueva)
    {
        if (Math.Abs((horarioAnterior - horarioNuevo).TotalMinutes) >= 1)
        {
            var tipo = horarioNuevo < horarioAnterior
                ? TipoCambioOperativo.Adelanto
                : TipoCambioOperativo.Retraso;

            await _cambioOperativoServicio.RegistrarCambioDesdeEdicionAsync(
                vuelo.Id,
                vuelo.NumeroVuelo,
                tipo,
                causa,
                horarioAnterior.ToString("g"),
                horarioNuevo.ToString("g"));
        }

        if (!string.Equals(puertaAnterior, puertaNueva, StringComparison.OrdinalIgnoreCase))
        {
            await _cambioOperativoServicio.RegistrarCambioDesdeEdicionAsync(
                vuelo.Id,
                vuelo.NumeroVuelo,
                TipoCambioOperativo.CambioPuerta,
                causa,
                puertaAnterior ?? "(sin asignar)",
                puertaNueva ?? "(sin asignar)");
        }

        if (aerolineaAnteriorId != aerolineaNuevaId)
        {
            await _cambioOperativoServicio.RegistrarCambioDesdeEdicionAsync(
                vuelo.Id,
                vuelo.NumeroVuelo,
                TipoCambioOperativo.CambioAerolinea,
                causa,
                aerolineaAnteriorNombre,
                aerolineaNuevaNombre);
        }

        if (cambioRuta)
        {
            await _cambioOperativoServicio.RegistrarCambioDesdeEdicionAsync(
                vuelo.Id,
                vuelo.NumeroVuelo,
                TipoCambioOperativo.CambioRuta,
                causa,
                rutaAnterior,
                rutaNueva);
        }
    }

    public async Task EliminarAsync(int id)
    {
        _logger.LogInformation("Iniciando eliminación de vuelo con id {Id}", id);
        var vuelo = await _unidadDeTrabajo.Vuelos.ObtenerPorIdAsync(id);

        if (vuelo == null)
        {
            _logger.LogWarning("No se encontró el vuelo con id {Id} para eliminar.", id);
            return;
        }

        _unidadDeTrabajo.Vuelos.Eliminar(vuelo);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Eliminar", "Vuelos", $"Se eliminó el vuelo {vuelo.NumeroVuelo} (id {vuelo.Id}).");
    }

    private async Task ValidarReferenciasAsync(
        int aerolineaId,
        int aeropuertoOrigenId,
        int aeropuertoDestinoId)
    {
        var aerolinea = await _unidadDeTrabajo.Aerolineas.ObtenerPorIdAsync(aerolineaId);
        if (aerolinea == null)
        {
            _logger.LogError("Error de validación de referencias: No existe aerolínea {Id}.", aerolineaId);
            throw new ExcepcionRecursoNoEncontrado($"No existe la aerolínea con id {aerolineaId}.");
        }

        var origen = await _unidadDeTrabajo.Aeropuertos.ObtenerPorIdAsync(aeropuertoOrigenId);
        if (origen == null)
        {
            _logger.LogError("Error de validación de referencias: No existe aeropuerto origen {Id}.", aeropuertoOrigenId);
            throw new ExcepcionRecursoNoEncontrado(
                $"No existe el aeropuerto de origen con id {aeropuertoOrigenId}.");
        }

        var destino = await _unidadDeTrabajo.Aeropuertos.ObtenerPorIdAsync(aeropuertoDestinoId);
        if (destino == null)
        {
            _logger.LogError("Error de validación de referencias: No existe aeropuerto destino {Id}.", aeropuertoDestinoId);
            throw new ExcepcionRecursoNoEncontrado(
                $"No existe el aeropuerto de destino con id {aeropuertoDestinoId}.");
        }
    }

    private async Task<string?> ValidarPuertaDisponibleAsync(
        string? puertaSolicitada,
        int aeropuertoId,
        DateTime horario,
        int? excluirVueloId = null)
    {
        if (string.IsNullOrWhiteSpace(puertaSolicitada))
            return null;

        var nombrePuerta = puertaSolicitada.Trim().ToUpperInvariant();
        var puerta = (await _unidadDeTrabajo.Puertas.ObtenerTodosAsync())
            .FirstOrDefault(p => p.AeropuertoId == aeropuertoId &&
                p.Nombre.Equals(nombrePuerta, StringComparison.OrdinalIgnoreCase));

        if (puerta is null)
            throw new ExcepcionDeValidacion(
                $"La puerta '{nombrePuerta}' no está disponible para el aeropuerto seleccionado.");

        var vuelos = await _unidadDeTrabajo.Vuelos.ObtenerTodosConDetalleAsync();
        var puertaOcupada = vuelos.Any(v =>
            (!excluirVueloId.HasValue || v.Id != excluirVueloId.Value) &&
            v.AeropuertoOrigenId == aeropuertoId &&
            string.Equals(v.Puerta, puerta.Nombre, StringComparison.OrdinalIgnoreCase) &&
            v.EstadoVuelo is not null &&
            !v.EstadoVuelo.EsFinal() &&
            Math.Abs((v.HorarioProgramado - horario).TotalMinutes) < 120);

        if (puertaOcupada)
            throw new ExcepcionDeValidacion(
                $"La puerta '{nombrePuerta}' no está disponible para ese horario.");

        return puerta.Nombre;
    }
}
