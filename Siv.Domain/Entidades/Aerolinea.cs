namespace Siv.Domain.Entidades;

public class Aerolinea : EntidadBase
{
    protected Aerolinea()
    {
        Codigo = string.Empty;
        Nombre = string.Empty;
    }

    public Aerolinea(string codigo, string nombre)
    {
        ActualizarDatos(codigo, nombre);
    }

    public string Codigo { get; private set; } = string.Empty;

    public string Nombre { get; private set; } = string.Empty;

    public void ActualizarDatos(string codigo, string nombre)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código de la aerolínea es obligatorio.", nameof(codigo));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la aerolínea es obligatorio.", nameof(nombre));

        Codigo = codigo.Trim().ToUpperInvariant();
        Nombre = nombre.Trim();
    }
}
