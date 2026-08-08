using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class VueloMapeador
{
    public static VueloDto ADto(this Vuelo entidad)
    {
        return new VueloDto
        {
            VueloId = entidad.Id,
            NumeroVuelo = entidad.NumeroVuelo,
            AerolineaId = entidad.AerolineaId,
            AerolineaNombre = entidad.Aerolinea?.Nombre,
            AeropuertoOrigenId = entidad.AeropuertoOrigenId,
            AeropuertoOrigenNombre = entidad.AeropuertoOrigen?.Nombre,
            AeropuertoDestinoId = entidad.AeropuertoDestinoId,
            AeropuertoDestinoNombre = entidad.AeropuertoDestino?.Nombre,
            HorarioProgramado = entidad.HorarioProgramado,
            Puerta = entidad.Puerta,
            EstadoVueloId = entidad.EstadoVueloId,
            EstadoVueloNombre = entidad.EstadoVuelo?.Nombre,
            FechaCreacion = entidad.FechaCreacion,
            NivelVisibilidad = entidad.NivelVisibilidad.ToString()
        };
    }
}
