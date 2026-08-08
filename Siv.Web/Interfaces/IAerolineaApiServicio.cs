using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface IAerolineaApiServicio
{
    Task<List<AerolineaViewModel>> ObtenerTodosAsync();
    Task<AerolineaViewModel?> ObtenerPorIdAsync(int id);
}
