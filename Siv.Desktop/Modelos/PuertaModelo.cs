namespace Siv.Desktop.Modelos;

public class PuertaModelo
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public int AeropuertoId { get; set; }
    public string Estado { get; set; } = string.Empty;
}
