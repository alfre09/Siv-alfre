using Siv.Web.Modelos;

namespace Siv.Web.Interfaces;

public interface IReservaApiServicio
{
    Task<ReservaViewModel> CrearAsync(int vueloId);
    Task<List<ReservaViewModel>> ObtenerMisReservasAsync();
    Task CancelarAsync(int reservaId);
}
