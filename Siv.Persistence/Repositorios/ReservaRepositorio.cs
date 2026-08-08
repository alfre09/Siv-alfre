using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Siv.Domain.Entidades;
using Siv.Domain.Enums;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class ReservaRepositorio : RepositorioBase<Reserva>, IReservaRepositorio
{
    public ReservaRepositorio(SivDbContext contexto, ILogger<ReservaRepositorio> logger)
        : base(contexto, logger)
    {
    }

    public async Task<List<Reserva>> ObtenerPorUsuarioAsync(string usuario)
    {
        return await ConjuntoDatos
            .Include(r => r.Vuelo)
                .ThenInclude(v => v!.AeropuertoOrigen)
            .Include(r => r.Vuelo)
                .ThenInclude(v => v!.AeropuertoDestino)
            .Where(r => r.Usuario == usuario)
            .OrderByDescending(r => r.FechaReserva)
            .ToListAsync();
    }

    public async Task<Reserva?> ObtenerActivaAsync(string usuario, int vueloId)
    {
        return await ConjuntoDatos.FirstOrDefaultAsync(r =>
            r.Usuario == usuario &&
            r.VueloId == vueloId &&
            r.Estado == EstadoReserva.Activa);
    }
}
