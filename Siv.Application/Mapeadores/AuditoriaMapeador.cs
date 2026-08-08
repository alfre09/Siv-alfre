using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class AuditoriaMapeador
{
    public static AuditoriaDto ADto(this Auditoria entidad)
    {
        return new AuditoriaDto
        {
            AuditoriaId = entidad.Id,
            Accion = entidad.Accion,
            Tabla = entidad.Tabla,
            Descripcion = entidad.Descripcion,
            Fecha = entidad.Fecha
        };
    }
}
