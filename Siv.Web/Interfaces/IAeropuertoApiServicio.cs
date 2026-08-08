using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface IAeropuertoApiServicio
{
    Task<List<AeropuertoViewModel>> ObtenerTodosAsync();
    Task<AeropuertoViewModel?> ObtenerPorIdAsync(int id);
}
