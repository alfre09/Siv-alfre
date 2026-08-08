using Siv.Domain.Entidades;

namespace Siv.Domain.Repositorios;

public interface IAuditoriaRepositorio : IRepositorioBase<Auditoria>
{
    Task<List<Auditoria>> ObtenerPorTablaAsync(string tabla);
}
