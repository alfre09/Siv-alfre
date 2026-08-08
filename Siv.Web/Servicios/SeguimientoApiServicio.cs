using System.Net.Http.Json;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class SeguimientoApiServicio : ApiServicioBase, ISeguimientoApiServicio
{
    public SeguimientoApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<SeguimientoViewModel> CrearAsync(CrearSeguimientoViewModel modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/seguimientos", new
        {
            usuario = modelo.Usuario,
            vueloId = modelo.VueloId
        });

        return await LeerRespuestaExitosaAsync<SeguimientoViewModel>(respuesta);
    }

    public async Task EliminarAsync(int id)
    {
        var respuesta = await ClienteHttp.DeleteAsync($"api/seguimientos/{id}");
        await AsegurarExitoAsync(respuesta);
    }
}
