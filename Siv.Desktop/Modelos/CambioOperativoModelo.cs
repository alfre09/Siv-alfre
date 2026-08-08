namespace Siv.Desktop.Modelos;

public class CambioOperativoModelo
{
    public int CambioOperativoId { get; set; }
    public int VueloId { get; set; }
    public string TipoCambio { get; set; } = string.Empty;
    public string Causa { get; set; } = string.Empty;
    public DateTime FechaCambio { get; set; }
}

public class RegistrarRetrasoOAdelantoModelo
{
    public int VueloId { get; set; }
    public bool EsAdelanto { get; set; }
    public DateTime NuevoHorario { get; set; }
    public string Causa { get; set; } = string.Empty;
}

public class RegistrarCambioPuertaModelo
{
    public int VueloId { get; set; }
    public string NuevaPuerta { get; set; } = string.Empty;
    public string Causa { get; set; } = string.Empty;
}

public class RegistrarCambioEstadoModelo
{
    public int VueloId { get; set; }
    public int NuevoEstadoVueloId { get; set; }
    public string Causa { get; set; } = string.Empty;
}

public class RegistrarCancelacionModelo
{
    public int VueloId { get; set; }
    public string Causa { get; set; } = string.Empty;
}
