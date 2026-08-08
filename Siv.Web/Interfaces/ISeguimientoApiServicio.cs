using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface ISeguimientoApiServicio
{
    Task<SeguimientoViewModel> CrearAsync(CrearSeguimientoViewModel modelo);
    Task EliminarAsync(int id);
}
