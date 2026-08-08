using System.Net.Http;
using System.Net.Http.Json;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class AerolineaApiServicio : ApiServicioBase, IAerolineaApiServicio
{
    public AerolineaApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<AerolineaModelo>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/aerolineas");
        return await LeerRespuestaExitosaAsync<List<AerolineaModelo>>(respuesta);
    }

    public async Task<AerolineaModelo?> ObtenerPorIdAsync(int id)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/aerolineas/{id}");

        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        return await LeerRespuestaExitosaAsync<AerolineaModelo>(respuesta);
    }

    public async Task<AerolineaModelo> CrearAsync(CrearAerolineaModelo modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/aerolineas", modelo);
        return await LeerRespuestaExitosaAsync<AerolineaModelo>(respuesta);
    }

    public async Task ActualizarAsync(ActualizarAerolineaModelo modelo)
    {
        var respuesta = await ClienteHttp.PutAsJsonAsync($"api/aerolineas/{modelo.AerolineaId}", modelo);
        await AsegurarExitoAsync(respuesta);
    }

    public async Task EliminarAsync(int id)
    {
        var respuesta = await ClienteHttp.DeleteAsync($"api/aerolineas/{id}");
        await AsegurarExitoAsync(respuesta);
    }
}
