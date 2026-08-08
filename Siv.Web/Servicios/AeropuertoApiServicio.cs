using System.Net;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class AeropuertoApiServicio : ApiServicioBase, IAeropuertoApiServicio
{
    public AeropuertoApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<List<AeropuertoViewModel>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/aeropuertos");
        return await LeerRespuestaExitosaAsync<List<AeropuertoViewModel>>(respuesta);
    }

    public async Task<AeropuertoViewModel?> ObtenerPorIdAsync(int id)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/aeropuertos/{id}");

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;

        return await LeerRespuestaExitosaAsync<AeropuertoViewModel>(respuesta);
    }
}
