using Siv.Application.Dtos;
using Siv.Domain.Enums;

namespace Siv.Application.Interfaces;

public interface ICambioOperativoServicio
{
    Task<List<CambioOperativoDto>> ObtenerTodosAsync();
    Task<List<CambioOperativoDto>> ObtenerPorVueloAsync(int vueloId, string? rolUsuario = null);
    Task<CambioOperativoDto> RegistrarRetrasoOAdelantoAsync(RegistrarRetrasoOAdelantoDto dto);
    Task<CambioOperativoDto> RegistrarCambioPuertaAsync(RegistrarCambioPuertaDto dto);
    Task<CambioOperativoDto> RegistrarCambioEstadoAsync(RegistrarCambioEstadoDto dto);
    Task<CambioOperativoDto> RegistrarCancelacionAsync(RegistrarCancelacionDto dto);

    /// <summary>
    /// Deja constancia de un cambio operativo cuyo efecto (horario, puerta, etc.)
    /// ya fue aplicado y guardado por quien llama (ej. VueloServicio.ActualizarAsync).
    /// Crea el registro de historial, la auditoría y las notificaciones a los
    /// usuarios que siguen el vuelo, sin volver a tocar la entidad Vuelo.
    /// </summary>
    Task<CambioOperativoDto> RegistrarCambioDesdeEdicionAsync(
        int vueloId, string numeroVuelo, TipoCambioOperativo tipo, string causa,
        string valorAnterior, string valorNuevo);
}
