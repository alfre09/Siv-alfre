using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class NotificacionRepositorio : RepositorioBase<Notificacion>, INotificacionRepositorio
{
    public NotificacionRepositorio(SivDbContext contexto, ILogger<NotificacionRepositorio> logger) : base(contexto, logger)
    {
    }

    public async Task<List<Notificacion>> ObtenerPorUsuarioAsync(string usuario)
    {
        _logger.LogInformation("Obteniendo notificaciones para el usuario: {Usuario}", usuario);
        return await ConjuntoDatos
            .Where(n => n.Usuario == usuario)
            .ToListAsync();
    }

    public async Task<List<Notificacion>> ObtenerPorVueloAsync(int vueloId)
    {
        _logger.LogInformation("Obteniendo notificaciones para el vuelo: {VueloId}", vueloId);
        return await ConjuntoDatos
            .Where(n => n.VueloId == vueloId)
            .ToListAsync();
    }

    public async Task AgregarRangoAsync(IEnumerable<Notificacion> notificaciones)
    {
        _logger.LogInformation("Agregando un rango de notificaciones");
        await ConjuntoDatos.AddRangeAsync(notificaciones);
    }
}
