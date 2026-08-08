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

    public async Task<SeguimientoViewModel?> ObtenerPorVueloYUsuarioAsync(int vueloId, string usuario)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/seguimientos/{vueloId}/verificar?usuario={Uri.EscapeDataString(usuario)}");
        
        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        return await LeerRespuestaExitosaAsync<SeguimientoViewModel>(respuesta);
    }
}
