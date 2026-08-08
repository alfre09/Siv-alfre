using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface ICambioOperativoApiServicio
{
    Task<List<CambioOperativoViewModel>> ObtenerPorVueloAsync(int vueloId);
}
