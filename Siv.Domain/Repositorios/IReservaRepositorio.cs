using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IReservaRepositorio : IRepositorioBase<Reserva>
{
    Task<List<Reserva>> ObtenerPorUsuarioAsync(string usuario);
    Task<Reserva?> ObtenerActivaAsync(string usuario, int vueloId);
}
