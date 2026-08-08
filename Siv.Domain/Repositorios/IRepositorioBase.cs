namespace Siv.Domain.Repositorios;

public interface IRepositorioBase<TEntidad> where TEntidad : Entidades.EntidadBase
{
    Task<TEntidad?> ObtenerPorIdAsync(int id);
    Task<List<TEntidad>> ObtenerTodosAsync();
    Task AgregarAsync(TEntidad entidad);
    void Actualizar(TEntidad entidad);
    void Eliminar(TEntidad entidad);
}
