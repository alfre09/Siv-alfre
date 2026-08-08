using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IAerolineaRepositorio : IRepositorioBase<Aerolinea>
{
    Task<bool> ExisteCodigoAsync(string codigo);
    Task<bool> TieneVuelosAsociadosAsync(int aerolineaId);
}
