using Siv.Domain.Enums;
using Siv.Domain.Excepciones;

namespace Siv.Domain.Entidades;

public class Vuelo : EntidadBase
{
    protected Vuelo()
    {
        NumeroVuelo = string.Empty;
    }

    public Vuelo(
        string numeroVuelo,
        int aerolineaId,
        int aeropuertoOrigenId,
        int aeropuertoDestinoId,
        DateTime horarioProgramado,
        int estadoVueloId,
        string? puerta = null,
        NivelVisibilidad nivelVisibilidad = NivelVisibilidad.Publico)
    {
        if (string.IsNullOrWhiteSpace(numeroVuelo))
            throw new ArgumentException("El vuelo debe tener un número de vuelo.", nameof(numeroVuelo));

        if (aeropuertoOrigenId == aeropuertoDestinoId)
            throw new ExcepcionDeDominio("El aeropuerto de origen y destino no pueden ser el mismo.");

        NumeroVuelo = numeroVuelo.Trim();
        AerolineaId = aerolineaId;
        AeropuertoOrigenId = aeropuertoOrigenId;
        AeropuertoDestinoId = aeropuertoDestinoId;
        HorarioProgramado = horarioProgramado;
        EstadoVueloId = estadoVueloId;
        Puerta = puerta;
        NivelVisibilidad = nivelVisibilidad;
        FechaCreacion = DateTime.UtcNow;
    }

    public string NumeroVuelo { get; private set; }

    public int AerolineaId { get; private set; }

    public Aerolinea? Aerolinea { get; private set; }

    public int AeropuertoOrigenId { get; private set; }

    public Aeropuerto? AeropuertoOrigen { get; private set; }

    public int AeropuertoDestinoId { get; private set; }

    public Aeropuerto? AeropuertoDestino { get; private set; }

    public DateTime HorarioProgramado { get; private set; }

    public string? Puerta { get; private set; }

    public int EstadoVueloId { get; private set; }

    public EstadoVuelo? EstadoVuelo { get; private set; }

    public NivelVisibilidad NivelVisibilidad { get; private set; }

    public DateTime FechaCreacion { get; private set; }

    public void ActualizarProgramacion(
        string numeroVuelo,
        int aerolineaId,
        int aeropuertoOrigenId,
        int aeropuertoDestinoId,
        DateTime horarioProgramado,
        string? puerta,
        NivelVisibilidad? nivelVisibilidad = null)
    {
        if (string.IsNullOrWhiteSpace(numeroVuelo))
            throw new ArgumentException("El vuelo debe tener un número de vuelo.", nameof(numeroVuelo));

        if (aeropuertoOrigenId == aeropuertoDestinoId)
            throw new ExcepcionDeDominio("El aeropuerto de origen y destino no pueden ser el mismo.");

        NumeroVuelo = numeroVuelo.Trim();
        AerolineaId = aerolineaId;
        AeropuertoOrigenId = aeropuertoOrigenId;
        AeropuertoDestinoId = aeropuertoDestinoId;
        HorarioProgramado = horarioProgramado;
        Puerta = puerta;

        if (nivelVisibilidad.HasValue)
            NivelVisibilidad = nivelVisibilidad.Value;
    }

    public (string ValorAnterior, string ValorNuevo) AplicarCambioDeHorario(DateTime nuevoHorario)
    {
        AsegurarQueNoEsteEnEstadoFinal();

        var valorAnterior = HorarioProgramado.ToString("g");
        HorarioProgramado = nuevoHorario;
        return (valorAnterior, HorarioProgramado.ToString("g"));
    }

    public (string ValorAnterior, string ValorNuevo) AplicarCambioDePuerta(string nuevaPuerta)
    {
        if (string.IsNullOrWhiteSpace(nuevaPuerta))
            throw new ExcepcionDeDominio("Debes indicar la nueva puerta.");

        AsegurarQueNoEsteEnEstadoFinal();

        var valorAnterior = Puerta ?? "(sin asignar)";
        Puerta = nuevaPuerta.Trim();
        return (valorAnterior, Puerta);
    }

    public (string ValorAnterior, string ValorNuevo, int EstadoAnteriorId) AplicarCambioDeEstado(EstadoVuelo estadoNuevo)
    {
        AsegurarQueNoEsteEnEstadoFinal();

        if (EstadoVuelo is null)
            throw new ExcepcionDeDominio("El vuelo no tiene un estado actual cargado.");

        CicloDeVidaVuelo.ValidarTransicion(EstadoVuelo, estadoNuevo);

        var valorAnterior = EstadoVuelo.Nombre;
        var estadoAnteriorId = EstadoVueloId;

        EstadoVueloId = estadoNuevo.Id;
        EstadoVuelo = estadoNuevo;

        return (valorAnterior, estadoNuevo.Nombre, estadoAnteriorId);
    }

    public void ValidarQueAdmiteCambioOperativo()
    {
        AsegurarQueNoEsteEnEstadoFinal();
    }

    private void AsegurarQueNoEsteEnEstadoFinal()
    {
        if (EstadoVuelo is null)
            return;

        if (EstadoVuelo.EsFinal())
            throw new ExcepcionDeDominio(
                $"El vuelo está en estado '{EstadoVuelo.Nombre}' y no se pueden registrar más cambios operativos sobre él.");
    }
}
