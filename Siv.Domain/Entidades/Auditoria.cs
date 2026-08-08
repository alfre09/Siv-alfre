namespace Siv.Domain.Entidades;

public class Auditoria : EntidadBase
{
    protected Auditoria()
    {
        Accion = string.Empty;
        Tabla = string.Empty;
        Descripcion = string.Empty;
        Usuario = string.Empty;
    }

    public Auditoria(string accion, string tabla, string descripcion, string usuario, int? registroId = null, string? valorAnterior = null, string? valorNuevo = null)
    {
        if (string.IsNullOrWhiteSpace(accion))
            throw new ArgumentException("La auditoría debe indicar una acción.", nameof(accion));

        if (string.IsNullOrWhiteSpace(tabla))
            throw new ArgumentException("La auditoría debe indicar la tabla afectada.", nameof(tabla));

        Accion = accion.Trim();
        Tabla = tabla.Trim();
        Descripcion = descripcion?.Trim() ?? string.Empty;
        Usuario = usuario?.Trim() ?? "Sistema";
        RegistroId = registroId;
        ValorAnterior = valorAnterior;
        ValorNuevo = valorNuevo;
        Fecha = DateTime.UtcNow;
    }

    public string Accion { get; private set; }

    public string Tabla { get; private set; }

    public string Descripcion { get; private set; }
    
    public string Usuario { get; private set; }
    
    public int? RegistroId { get; private set; }
    
    public string? ValorAnterior { get; private set; }
    
    public string? ValorNuevo { get; private set; }

    public DateTime Fecha { get; private set; }
}
