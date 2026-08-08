using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class EstadoVueloMapeador
{
    public static EstadoVueloDto ADto(this EstadoVuelo entidad)
    {
        return new EstadoVueloDto
        {
            EstadoVueloId = entidad.Id,
            Nombre = entidad.Nombre
        };
    }
}
