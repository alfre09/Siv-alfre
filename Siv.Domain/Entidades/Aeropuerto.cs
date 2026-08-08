namespace Siv.Domain.Entidades;

public class Aeropuerto : EntidadBase
{
    protected Aeropuerto()
    {
        Codigo = string.Empty;
        Nombre = string.Empty;
        Ciudad = string.Empty;
        Pais = string.Empty;
    }

    public Aeropuerto(string codigo, string nombre, string ciudad, string pais)
    {
        ActualizarDatos(codigo, nombre, ciudad, pais);
    }

    public string Codigo { get; private set; } = string.Empty;

    public string Nombre { get; private set; } = string.Empty;

    public string Ciudad { get; private set; } = string.Empty;

    public string Pais { get; private set; } = string.Empty;

    public void ActualizarDatos(string codigo, string nombre, string ciudad, string pais)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código del aeropuerto es obligatorio.", nameof(codigo));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del aeropuerto es obligatorio.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(ciudad))
            throw new ArgumentException("La ciudad del aeropuerto es obligatoria.", nameof(ciudad));

        if (string.IsNullOrWhiteSpace(pais))
            throw new ArgumentException("El país del aeropuerto es obligatorio.", nameof(pais));

        Codigo = codigo.Trim().ToUpperInvariant();
        Nombre = nombre.Trim();
        Ciudad = ciudad.Trim();
        Pais = pais.Trim();
    }
}
