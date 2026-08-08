using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class AeropuertoMapeador
{
    public static AeropuertoDto ADto(this Aeropuerto entidad)
    {
        return new AeropuertoDto
        {
            AeropuertoId = entidad.Id,
            Codigo = entidad.Codigo,
            Nombre = entidad.Nombre,
            Ciudad = entidad.Ciudad,
            Pais = entidad.Pais
        };
    }
}
