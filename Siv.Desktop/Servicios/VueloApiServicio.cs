using System.Net.Http;
using System.Net.Http.Json;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class VueloApiServicio : ApiServicioBase, IVueloApiServicio
{
    public VueloApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<VueloModelo>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/vuelos");
        return await LeerRespuestaExitosaAsync<List<VueloModelo>>(respuesta);
    }

    public async Task<VueloModelo?> ObtenerPorIdAsync(int id)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/vuelos/{id}");

        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        return await LeerRespuestaExitosaAsync<VueloModelo>(respuesta);
    }

    public async Task<VueloModelo> CrearAsync(CrearVueloModelo modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/vuelos", modelo);
        return await LeerRespuestaExitosaAsync<VueloModelo>(respuesta);
    }

    public async Task ActualizarAsync(ActualizarVueloModelo modelo)
    {
        var respuesta = await ClienteHttp.PutAsJsonAsync($"api/vuelos/{modelo.VueloId}", modelo);
        await AsegurarExitoAsync(respuesta);
    }

    public async Task EliminarAsync(int id)
    {
        var respuesta = await ClienteHttp.DeleteAsync($"api/vuelos/{id}");
        await AsegurarExitoAsync(respuesta);
    }
}
