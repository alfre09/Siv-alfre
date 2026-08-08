using System.ComponentModel.DataAnnotations;

namespace Siv.Web.Modelos;

public class VueloViewModel
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


public class DetalleVueloViewModel
{
    public VueloViewModel Vuelo { get; set; } = new();
    public List<CambioOperativoViewModel> Cambios { get; set; } = new();
    public bool EstaSiguiendo { get; set; }
    public int? SeguimientoId { get; set; }
}
