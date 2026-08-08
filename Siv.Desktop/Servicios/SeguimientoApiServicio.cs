using System.Net.Http;
using System.Net.Http.Json;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class SeguimientoApiServicio : ApiServicioBase, ISeguimientoApiServicio
{
    public SeguimientoApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<SeguimientoModelo>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/seguimientos");
        return await LeerRespuestaExitosaAsync<List<SeguimientoModelo>>(respuesta);
    }

    public async Task<SeguimientoModelo?> ObtenerPorIdAsync(int id)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/seguimientos/{id}");

        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        return await LeerRespuestaExitosaAsync<SeguimientoModelo>(respuesta);
    }

    public async Task<SeguimientoModelo> CrearAsync(CrearSeguimientoModelo modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/seguimientos", modelo);
        return await LeerRespuestaExitosaAsync<SeguimientoModelo>(respuesta);
    }

    public async Task ActualizarAsync(ActualizarSeguimientoModelo modelo)
    {
        var respuesta = await ClienteHttp.PutAsJsonAsync($"api/seguimientos/{modelo.SeguimientoId}", modelo);
        await AsegurarExitoAsync(respuesta);
    }

    public async Task EliminarAsync(int id)
    {
        var respuesta = await ClienteHttp.DeleteAsync($"api/seguimientos/{id}");
        await AsegurarExitoAsync(respuesta);
    }
}
