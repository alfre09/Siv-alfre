namespace Siv.Web.Modelos;

public class AuditoriaViewModel
{
    public int AuditoriaId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string Tabla { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
