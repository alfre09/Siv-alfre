using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface IAuditoriaApiServicio
{
    Task<List<AuditoriaViewModel>> ObtenerTodosAsync();
}
