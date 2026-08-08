namespace Siv.Domain.Entidades;

public class Seguimiento : EntidadBase
{
    protected Seguimiento()
    {
        Usuario = string.Empty;
    }

    public Seguimiento(string usuario, int vueloId)
    {
        if (string.IsNullOrWhiteSpace(usuario))
            throw new ArgumentException("El seguimiento debe estar asociado a un usuario.", nameof(usuario));

        Usuario = usuario.Trim();
        VueloId = vueloId;
        FechaSeguimiento = DateTime.UtcNow;
    }

    public string Usuario { get; private set; }

    public int VueloId { get; private set; }

    public Vuelo? Vuelo { get; private set; }

    public DateTime FechaSeguimiento { get; private set; }

    public void Reasignar(string usuario, int vueloId)
    {
        if (string.IsNullOrWhiteSpace(usuario))
            throw new ArgumentException("El seguimiento debe estar asociado a un usuario.", nameof(usuario));

        Usuario = usuario.Trim();
        VueloId = vueloId;
    }
}
