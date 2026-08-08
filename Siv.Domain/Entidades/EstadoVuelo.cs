namespace Siv.Domain.Entidades;

public class EstadoVuelo : EntidadBase
{
    public const string Programado = "Programado";
    public const string Embarcando = "Embarcando";
    public const string EnVuelo = "En Vuelo";
    public const string Aterrizado = "Aterrizado";
    public const string Cancelado = "Cancelado";

    public static readonly string[] SecuenciaOperativa =
    {
        Programado,
        Embarcando,
        EnVuelo,
        Aterrizado
    };

    protected EstadoVuelo()
    {
        Nombre = string.Empty;
    }

    public EstadoVuelo(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del estado de vuelo es obligatorio.", nameof(nombre));

        Nombre = nombre.Trim();
    }

    public string Nombre { get; private set; }

    public bool EsFinal()
    {
        return Nombre.Equals(Cancelado, StringComparison.OrdinalIgnoreCase)
            || Nombre.Equals(SecuenciaOperativa[^1], StringComparison.OrdinalIgnoreCase);
    }

    public bool EsPrimerEstado()
    {
        return Nombre.Equals(SecuenciaOperativa[0], StringComparison.OrdinalIgnoreCase);
    }

    public bool EsCancelado()
    {
        return Nombre.Equals(Cancelado, StringComparison.OrdinalIgnoreCase);
    }
}
