namespace Siv.Web.Modelos;

public class ReservaViewModel
{
    public int ReservaId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int VueloId { get; set; }
    public string NumeroVuelo { get; set; } = string.Empty;
    public string? Origen { get; set; }
    public string? Destino { get; set; }
    public DateTime HorarioProgramado { get; set; }
    public DateTime FechaReserva { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class DisponiblesViewModel
{
    public List<VueloViewModel> Vuelos { get; set; } = new();
    public List<AeropuertoViewModel> Aeropuertos { get; set; } = new();
    public int? OrigenId { get; set; }
    public int? DestinoId { get; set; }
    public DateTime? Fecha { get; set; }
}
