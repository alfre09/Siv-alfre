namespace Siv.Desktop.Modelos;

public class HistorialEstadoVueloModelo
{
    public int HistorialEstadoVueloId { get; set; }
    public int VueloId { get; set; }
    public int EstadoAnteriorId { get; set; }
    public string EstadoAnteriorNombre { get; set; } = string.Empty;
    public int EstadoNuevoId { get; set; }
    public string EstadoNuevoNombre { get; set; } = string.Empty;
    public DateTime FechaCambio { get; set; }
}
