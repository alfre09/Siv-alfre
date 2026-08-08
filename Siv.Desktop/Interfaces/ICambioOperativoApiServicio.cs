using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface ICambioOperativoApiServicio
{
    Task<List<CambioOperativoModelo>> ObtenerTodosAsync();
    Task<List<CambioOperativoModelo>> ObtenerPorVueloAsync(int vueloId);
    Task<CambioOperativoModelo> RegistrarRetrasoOAdelantoAsync(RegistrarRetrasoOAdelantoModelo modelo);
    Task<CambioOperativoModelo> RegistrarCambioPuertaAsync(RegistrarCambioPuertaModelo modelo);
    Task<CambioOperativoModelo> RegistrarCambioEstadoAsync(RegistrarCambioEstadoModelo modelo);
    Task<CambioOperativoModelo> RegistrarCancelacionAsync(RegistrarCancelacionModelo modelo);
}
