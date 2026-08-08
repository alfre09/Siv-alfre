using Microsoft.EntityFrameworkCore;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class PuertaRepositorio : IPuertaRepositorio
{
    private readonly SivDbContext _contexto;

    public PuertaRepositorio(SivDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<List<Puerta>> ObtenerTodosAsync()
    {
        return await _contexto.Puertas.ToListAsync();
    }

    public async Task<Puerta?> ObtenerPorNombreAsync(string nombre)
    {
        return await _contexto.Puertas.FirstOrDefaultAsync(p => p.Nombre == nombre);
    }
}
