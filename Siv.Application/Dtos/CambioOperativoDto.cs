namespace Siv.Application.Dtos;

public class CambioOperativoDto
{
    public int CambioOperativoId { get; set; }
    public int VueloId { get; set; }
    public string TipoCambio { get; set; } = string.Empty;
    public string Causa { get; set; } = string.Empty;
    public string ValorAnterior { get; set; } = string.Empty;
    public string ValorNuevo { get; set; } = string.Empty;
    public DateTime FechaCambio { get; set; }
}

public class RegistrarRetrasoOAdelantoDto
{
    public int VueloId { get; set; }
    public bool EsAdelanto { get; set; }
    public DateTime NuevoHorario { get; set; }
    public string Causa { get; set; } = string.Empty;
}

public class RegistrarCambioPuertaDto
{
    public int VueloId { get; set; }
    public string NuevaPuerta { get; set; } = string.Empty;
    public string Causa { get; set; } = string.Empty;
}

public class RegistrarCambioEstadoDto
{
    public int VueloId { get; set; }
    public int NuevoEstadoVueloId { get; set; }
    public string Causa { get; set; } = string.Empty;
}

public class RegistrarCancelacionDto
{
    public int VueloId { get; set; }
    public string Causa { get; set; } = string.Empty;
}
