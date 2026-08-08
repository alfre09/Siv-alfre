using System.Net.Http;
using System.Net.Http.Json;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class AeropuertoApiServicio : ApiServicioBase, IAeropuertoApiServicio
{
    public AeropuertoApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<AeropuertoModelo>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/aeropuertos");
        return await LeerRespuestaExitosaAsync<List<AeropuertoModelo>>(respuesta);
    }

    public async Task<AeropuertoModelo?> ObtenerPorIdAsync(int id)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/aeropuertos/{id}");

        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        return await LeerRespuestaExitosaAsync<AeropuertoModelo>(respuesta);
    }

    public async Task<AeropuertoModelo> CrearAsync(CrearAeropuertoModelo modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/aeropuertos", modelo);
        return await LeerRespuestaExitosaAsync<AeropuertoModelo>(respuesta);
    }

    public async Task ActualizarAsync(ActualizarAeropuertoModelo modelo)
    {
        var respuesta = await ClienteHttp.PutAsJsonAsync($"api/aeropuertos/{modelo.AeropuertoId}", modelo);
        await AsegurarExitoAsync(respuesta);
    }

    public async Task EliminarAsync(int id)
    {
        var respuesta = await ClienteHttp.DeleteAsync($"api/aeropuertos/{id}");
        await AsegurarExitoAsync(respuesta);
    }
}
