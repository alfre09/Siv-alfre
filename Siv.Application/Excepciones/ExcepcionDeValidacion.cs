namespace Siv.Application.Excepciones;

public class ExcepcionDeValidacion : Exception
{
    public ExcepcionDeValidacion(string mensaje) : base(mensaje)
    {
    }
}
