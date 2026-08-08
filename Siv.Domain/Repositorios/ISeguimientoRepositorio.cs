using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface ISeguimientoRepositorio : IRepositorioBase<Seguimiento>
{
    Task<bool> ExisteSeguimientoAsync(int vueloId, string usuario);
    Task<Seguimiento?> ObtenerPorVueloYUsuarioAsync(int vueloId, string usuario);
    Task<List<string>> ObtenerUsuariosInteresadosAsync(int vueloId);
}
