using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class SeguimientoMapeador
{
    public static SeguimientoDto ADto(this Seguimiento entidad)
    {
        return new SeguimientoDto
        {
            SeguimientoId = entidad.Id,
            Usuario = entidad.Usuario,
            VueloId = entidad.VueloId,
            FechaSeguimiento = entidad.FechaSeguimiento
        };
    }
}
