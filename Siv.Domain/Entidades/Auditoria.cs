namespace Siv.Domain.Entidades;

public class Auditoria : EntidadBase
{
    protected Auditoria()
    {
        Accion = string.Empty;
        Tabla = string.Empty;
        Descripcion = string.Empty;
    }

    public Auditoria(string accion, string tabla, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(accion))
            throw new ArgumentException("La auditoría debe indicar una acción.", nameof(accion));

        if (string.IsNullOrWhiteSpace(tabla))
            throw new ArgumentException("La auditoría debe indicar la tabla afectada.", nameof(tabla));

        Accion = accion.Trim();
        Tabla = tabla.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;
        Fecha = DateTime.UtcNow;
    }

    public string Accion { get; private set; }

    public string Tabla { get; private set; }

    public string Descripcion { get; private set; }

    public DateTime Fecha { get; private set; }
}
