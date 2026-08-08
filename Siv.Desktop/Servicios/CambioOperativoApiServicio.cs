using System.Net.Http;
using System.Net.Http.Json;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class CambioOperativoApiServicio : ApiServicioBase, ICambioOperativoApiServicio
{
    public CambioOperativoApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<CambioOperativoModelo>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/cambiosoperativos");
        return await LeerRespuestaExitosaAsync<List<CambioOperativoModelo>>(respuesta);
    }

    public async Task<List<CambioOperativoModelo>> ObtenerPorVueloAsync(int vueloId)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/cambiosoperativos/vuelo/{vueloId}");
        return await LeerRespuestaExitosaAsync<List<CambioOperativoModelo>>(respuesta);
    }

    public async Task<CambioOperativoModelo> RegistrarRetrasoOAdelantoAsync(RegistrarRetrasoOAdelantoModelo modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/cambiosoperativos/horario", modelo);
        return await LeerRespuestaExitosaAsync<CambioOperativoModelo>(respuesta);
    }

    public async Task<CambioOperativoModelo> RegistrarCambioPuertaAsync(RegistrarCambioPuertaModelo modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/cambiosoperativos/puerta", modelo);
        return await LeerRespuestaExitosaAsync<CambioOperativoModelo>(respuesta);
    }

    public async Task<CambioOperativoModelo> RegistrarCambioEstadoAsync(RegistrarCambioEstadoModelo modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/cambiosoperativos/estado", modelo);
        return await LeerRespuestaExitosaAsync<CambioOperativoModelo>(respuesta);
    }

    public async Task<CambioOperativoModelo> RegistrarCancelacionAsync(RegistrarCancelacionModelo modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/cambiosoperativos/cancelacion", modelo);
        return await LeerRespuestaExitosaAsync<CambioOperativoModelo>(respuesta);
    }
}
