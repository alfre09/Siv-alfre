using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface ICambioOperativoRepositorio : IRepositorioBase<CambioOperativo>
{
    Task<List<CambioOperativo>> ObtenerPorVueloAsync(int vueloId);
}
