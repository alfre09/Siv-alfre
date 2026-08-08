using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class NotificacionMapeador
{
    public static NotificacionDto ADto(this Notificacion entidad)
    {
        return new NotificacionDto
        {
            NotificacionId = entidad.Id,
            VueloId = entidad.VueloId,
            CambioOperativoId = entidad.CambioOperativoId,
            Usuario = entidad.Usuario,
            Mensaje = entidad.Mensaje,
            Leida = entidad.Leida,
            FechaEnvio = entidad.FechaEnvio
        };
    }
}
