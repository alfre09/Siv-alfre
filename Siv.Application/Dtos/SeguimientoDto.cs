namespace Siv.Application.Dtos;

public class SeguimientoDto
{
    public int SeguimientoId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int VueloId { get; set; }
    public DateTime FechaSeguimiento { get; set; }
}

public class CrearSeguimientoDto
{
    public string Usuario { get; set; } = string.Empty;
    public int VueloId { get; set; }
}

public class ActualizarSeguimientoDto
{
    public int SeguimientoId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int VueloId { get; set; }
}
