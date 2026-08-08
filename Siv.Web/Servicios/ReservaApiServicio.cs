using System.Net.Http.Json;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class ReservaApiServicio : ApiServicioBase, IReservaApiServicio
{
    public ReservaApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<ReservaViewModel> CrearAsync(int vueloId)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/reservas", new { vueloId });
        return await LeerRespuestaExitosaAsync<ReservaViewModel>(respuesta);
    }

    public async Task<List<ReservaViewModel>> ObtenerMisReservasAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/reservas/mis-reservas");
        return await LeerRespuestaExitosaAsync<List<ReservaViewModel>>(respuesta);
    }

    public async Task CancelarAsync(int reservaId)
    {
        var respuesta = await ClienteHttp.DeleteAsync($"api/reservas/{reservaId}");
        await AsegurarExitoAsync(respuesta);
    }
}
