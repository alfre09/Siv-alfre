namespace Siv.Desktop.Modelos;

public class VueloModelo
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
}

public class CrearVueloModelo
{
    public string NumeroVuelo { get; set; } = string.Empty;
    public int AerolineaId { get; set; }
    public int AeropuertoOrigenId { get; set; }
    public int AeropuertoDestinoId { get; set; }
    public DateTime HorarioProgramado { get; set; }
    public string? Puerta { get; set; }
}

public class ActualizarVueloModelo
{
    public int VueloId { get; set; }
    public string NumeroVuelo { get; set; } = string.Empty;
    public int AerolineaId { get; set; }
    public int AeropuertoOrigenId { get; set; }
    public int AeropuertoDestinoId { get; set; }
    public DateTime HorarioProgramado { get; set; }
    public string? Puerta { get; set; }
}
