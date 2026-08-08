using Siv.Domain.Enums;
using System.Text.Json.Serialization;

namespace Siv.Application.Dtos;

public class VueloDto
{
    public int VueloId { get; set; }
    public string NumeroVuelo { get; set; } = string.Empty;
    public int AerolineaId { get; set; }
    public string? AerolineaNombre { get; set; }
    public int AeropuertoOrigenId { get; set; }
    public string? AeropuertoOrigenNombre { get; set; }
    public int AeropuertoDestinoId { get; set; }
    public string? AeropuertoDestinoNombre { get; set; }
    public DateTime HorarioProgramado { get; set; }
    public string? Puerta { get; set; }
    public int EstadoVueloId { get; set; }
    public string? EstadoVueloNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string NivelVisibilidad { get; set; } = "Publico";
}

public class CrearVueloDto
{
    public string NumeroVuelo { get; set; } = string.Empty;
    public int AerolineaId { get; set; }
    public int AeropuertoOrigenId { get; set; }
    public int AeropuertoDestinoId { get; set; }
    public DateTime HorarioProgramado { get; set; }
    public string? Puerta { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NivelVisibilidad NivelVisibilidad { get; set; } = NivelVisibilidad.Publico;
}

public class ActualizarVueloDto
{
    public int VueloId { get; set; }
    public string NumeroVuelo { get; set; } = string.Empty;
    public int AerolineaId { get; set; }
    public int AeropuertoOrigenId { get; set; }
    public int AeropuertoDestinoId { get; set; }
    public DateTime HorarioProgramado { get; set; }
    public string? Puerta { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NivelVisibilidad NivelVisibilidad { get; set; } = NivelVisibilidad.Publico;

    /// <summary>
    /// Causa obligatoria cuando se modifica el horario o la puerta.
    /// </summary>
    public string? Causa { get; set; }
}
