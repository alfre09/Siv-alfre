using Siv.Domain.Enums;

namespace Siv.Domain.Entidades;

public class Reserva : EntidadBase
{
    protected Reserva()
    {
        Usuario = string.Empty;
    }

    public Reserva(string usuario, int vueloId)
    {
        if (string.IsNullOrWhiteSpace(usuario))
            throw new ArgumentException("La reserva debe tener un usuario.", nameof(usuario));
        if (vueloId <= 0)
            throw new ArgumentException("La reserva debe tener un vuelo válido.", nameof(vueloId));

        Usuario = usuario.Trim().ToLowerInvariant();
        VueloId = vueloId;
        FechaReserva = DateTime.UtcNow;
        Estado = EstadoReserva.Activa;
    }

    public string Usuario { get; private set; }
    public int VueloId { get; private set; }
    public Vuelo? Vuelo { get; private set; }
    public DateTime FechaReserva { get; private set; }
    public EstadoReserva Estado { get; private set; }

    public void Cancelar()
    {
        if (Estado == EstadoReserva.Cancelada)
            return;

        Estado = EstadoReserva.Cancelada;
    }

    public void AsociarVuelo(Vuelo vuelo)
    {
        Vuelo = vuelo ?? throw new ArgumentNullException(nameof(vuelo));
    }
}
