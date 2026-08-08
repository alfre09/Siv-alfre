using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class HistorialEstadoVueloMapeador
{
    public static HistorialEstadoVueloDto ADto(
        this HistorialEstadoVuelo entidad,
        string nombreEstadoAnterior,
        string nombreEstadoNuevo)
    {
        return new HistorialEstadoVueloDto
        {
            HistorialEstadoVueloId = entidad.Id,
            VueloId = entidad.VueloId,
            EstadoAnteriorId = entidad.EstadoAnteriorId,
            EstadoAnteriorNombre = nombreEstadoAnterior,
            EstadoNuevoId = entidad.EstadoNuevoId,
            EstadoNuevoNombre = nombreEstadoNuevo,
            FechaCambio = entidad.FechaCambio
        };
    }
}
