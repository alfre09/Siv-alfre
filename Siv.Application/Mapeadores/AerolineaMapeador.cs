using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class AerolineaMapeador
{
    public static AerolineaDto ADto(this Aerolinea entidad)
    {
        return new AerolineaDto
        {
            AerolineaId = entidad.Id,
            Codigo = entidad.Codigo,
            Nombre = entidad.Nombre
        };
    }
}
