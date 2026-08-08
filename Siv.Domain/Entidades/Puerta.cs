namespace Siv.Domain.Entidades;

public class Puerta : EntidadBase
{
    protected Puerta()
    {
        Nombre = string.Empty;
    }

    public Puerta(string nombre, int aeropuertoId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la puerta es obligatorio.", nameof(nombre));
            
        Nombre = nombre.Trim();
        AeropuertoId = aeropuertoId;
    }

    public string Nombre { get; private set; }
    public int AeropuertoId { get; private set; }
    
    // Relación opcional para navegación, si lo requiere EF Core
    public Aeropuerto? Aeropuerto { get; private set; }
}
