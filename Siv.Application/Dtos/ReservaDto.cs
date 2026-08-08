namespace Siv.Application.Dtos;

public class ReservaDto
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

public class CrearReservaDto
{
    public int VueloId { get; set; }
}
