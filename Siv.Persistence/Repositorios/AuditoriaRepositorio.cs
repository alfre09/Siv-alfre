using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class AuditoriaRepositorio : RepositorioBase<Auditoria>, IAuditoriaRepositorio
{
    public AuditoriaRepositorio(SivDbContext contexto, ILogger<AuditoriaRepositorio> logger) : base(contexto, logger)
    {
    }

    public async Task<List<Auditoria>> ObtenerPorTablaAsync(string tabla)
    {
        _logger.LogInformation("Obteniendo auditorías para la tabla: {Tabla}", tabla);
        return await ConjuntoDatos
            .Where(a => a.Tabla == tabla)
            .ToListAsync();
    }
}
