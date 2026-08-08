namespace Siv.Web.Modelos;

public class ExcepcionApi : Exception
{
    public int CodigoEstado { get; }

    public ExcepcionApi(int codigoEstado, string mensaje) : base(mensaje)
    {
        CodigoEstado = codigoEstado;
    }
}
