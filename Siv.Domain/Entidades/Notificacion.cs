namespace Siv.Domain.Entidades;

public class Notificacion : EntidadBase
{
    protected Notificacion()
    {
        Usuario = string.Empty;
        Mensaje = string.Empty;
    }

    public Notificacion(int vueloId, int cambioOperativoId, string usuario, string mensaje)
    {
        if (vueloId <= 0)
            throw new ArgumentException("La notificación debe estar asociada a un vuelo válido.", nameof(vueloId));

        if (cambioOperativoId <= 0)
            throw new ArgumentException("La notificación debe estar asociada a un cambio operativo válido.", nameof(cambioOperativoId));

        if (string.IsNullOrWhiteSpace(usuario))
            throw new ArgumentException("La notificación debe tener un usuario destinatario.", nameof(usuario));

        if (string.IsNullOrWhiteSpace(mensaje))
            throw new ArgumentException("La notificación debe tener un mensaje.", nameof(mensaje));

        VueloId = vueloId;
        CambioOperativoId = cambioOperativoId;
        Usuario = usuario.Trim();
        Mensaje = mensaje.Trim();
        Leida = false;
        FechaEnvio = DateTime.UtcNow;
    }

    public int VueloId { get; private set; }
    public Vuelo? Vuelo { get; private set; }

    public int? CambioOperativoId { get; private set; }
    public CambioOperativo? CambioOperativo { get; private set; }

    public string Usuario { get; private set; }
    public string Mensaje { get; private set; }
    public bool Leida { get; private set; }
    public DateTime FechaEnvio { get; private set; }

    public void MarcarComoLeida()
    {
        Leida = true;
    }
}
