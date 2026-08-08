using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class UsuarioRepositorio : RepositorioBase<Usuario>, IUsuarioRepositorio
{
    public UsuarioRepositorio(SivDbContext contexto, ILogger<UsuarioRepositorio> logger)
        : base(contexto, logger)
    {
    }

    public Task<Usuario?> ObtenerPorNombreAsync(string nombreUsuario)
    {
        var normalizado = nombreUsuario.Trim().ToLowerInvariant();
        return ConjuntoDatos.FirstOrDefaultAsync(u => u.NombreUsuario == normalizado);
    }
}
