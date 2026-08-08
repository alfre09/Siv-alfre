namespace Siv.Desktop.Modelos;

public class SeguimientoModelo
{
    public int SeguimientoId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int VueloId { get; set; }
    public DateTime FechaSeguimiento { get; set; }
}

public class CrearSeguimientoModelo
{
    public string Usuario { get; set; } = string.Empty;
    public int VueloId { get; set; }
}

public class ActualizarSeguimientoModelo
{
    public int SeguimientoId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int VueloId { get; set; }
}
