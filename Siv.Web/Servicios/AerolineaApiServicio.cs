using System.Net;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class AerolineaApiServicio : ApiServicioBase, IAerolineaApiServicio
{
    public AerolineaApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<List<AerolineaViewModel>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/aerolineas");
        return await LeerRespuestaExitosaAsync<List<AerolineaViewModel>>(respuesta);
    }

    public async Task<AerolineaViewModel?> ObtenerPorIdAsync(int id)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/aerolineas/{id}");

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;

        return await LeerRespuestaExitosaAsync<AerolineaViewModel>(respuesta);
    }
}
