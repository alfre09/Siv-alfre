namespace Siv.Web.Modelos;

public class CambioOperativoViewModel
{
    public int CambioOperativoId { get; set; }
    public int VueloId { get; set; }
    public string TipoCambio { get; set; } = string.Empty;
    public string Causa { get; set; } = string.Empty;
    public string ValorAnterior { get; set; } = string.Empty;
    public string ValorNuevo { get; set; } = string.Empty;
    public DateTime FechaCambio { get; set; }
}
