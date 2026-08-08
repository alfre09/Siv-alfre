namespace Siv.Application.Dtos;

public class NotificacionDto
{
    public int NotificacionId { get; set; }
    public int VueloId { get; set; }
    public int? CambioOperativoId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public bool Leida { get; set; }
    public DateTime FechaEnvio { get; set; }
}
