using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IAeropuertoRepositorio : IRepositorioBase<Aeropuerto>
{
    Task<bool> ExisteCodigoAsync(string codigo);
    Task<bool> TieneVuelosAsociadosAsync(int aeropuertoId);
}
