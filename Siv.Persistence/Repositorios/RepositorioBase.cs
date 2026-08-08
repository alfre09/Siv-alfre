using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class RepositorioBase<TEntidad> : IRepositorioBase<TEntidad> where TEntidad : EntidadBase
{
    protected readonly SivDbContext Contexto;
    protected readonly DbSet<TEntidad> ConjuntoDatos;
    protected readonly ILogger _logger;

    public RepositorioBase(SivDbContext contexto, ILogger logger)
    {
        Contexto = contexto;
        ConjuntoDatos = contexto.Set<TEntidad>();
        _logger = logger;
    }

    public virtual async Task<TEntidad?> ObtenerPorIdAsync(int id)
    {
        return await ConjuntoDatos.FindAsync(id);
    }

    public virtual async Task<List<TEntidad>> ObtenerTodosAsync()
    {
        return await ConjuntoDatos.ToListAsync();
    }

    public virtual async Task AgregarAsync(TEntidad entidad)
    {
        await ConjuntoDatos.AddAsync(entidad);
    }

    public virtual void Actualizar(TEntidad entidad)
    {
        ConjuntoDatos.Update(entidad);
    }

    public virtual void Eliminar(TEntidad entidad)
    {
        ConjuntoDatos.Remove(entidad);
    }
}
