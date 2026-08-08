using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface INotificacionRepositorio : IRepositorioBase<Notificacion>
{
    Task<List<Notificacion>> ObtenerPorUsuarioAsync(string usuario);
    Task<List<Notificacion>> ObtenerPorVueloAsync(int vueloId);
    Task AgregarRangoAsync(IEnumerable<Notificacion> notificaciones);
}
