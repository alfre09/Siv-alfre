namespace Siv.Domain.Entidades;

public class HistorialEstadoVuelo : EntidadBase
{
    protected HistorialEstadoVuelo()
    {
    }

    public HistorialEstadoVuelo(int vueloId, int estadoAnteriorId, int estadoNuevoId)
    {
        VueloId = vueloId;
        EstadoAnteriorId = estadoAnteriorId;
        EstadoNuevoId = estadoNuevoId;
        FechaCambio = DateTime.UtcNow;
    }

    public int VueloId { get; private set; }

    public Vuelo? Vuelo { get; private set; }

    public int EstadoAnteriorId { get; private set; }

    public int EstadoNuevoId { get; private set; }

    public DateTime FechaCambio { get; private set; }
}
