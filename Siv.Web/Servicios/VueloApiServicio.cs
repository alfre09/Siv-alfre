using System.Net;
using System.Net.Http.Json;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class VueloApiServicio : ApiServicioBase, IVueloApiServicio
{
    public VueloApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<List<VueloViewModel>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/vuelos");
        return await LeerRespuestaExitosaAsync<List<VueloViewModel>>(respuesta);
    }

    public async Task<List<VueloViewModel>> ObtenerDisponiblesAsync(int? origenId = null, int? destinoId = null, DateTime? fecha = null)
    {
        var parametros = new List<string>();
        if (origenId.HasValue) parametros.Add($"origenId={origenId.Value}");
        if (destinoId.HasValue) parametros.Add($"destinoId={destinoId.Value}");
        if (fecha.HasValue) parametros.Add($"fecha={Uri.EscapeDataString(fecha.Value.ToString("yyyy-MM-dd"))}");

        var ruta = "api/vuelos/disponibles";
        if (parametros.Count > 0)
            ruta += "?" + string.Join("&", parametros);

        var respuesta = await ClienteHttp.GetAsync(ruta);
        return await LeerRespuestaExitosaAsync<List<VueloViewModel>>(respuesta);
    }

    public async Task<VueloViewModel?> ObtenerPorIdAsync(int id)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/vuelos/{id}");

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;

        return await LeerRespuestaExitosaAsync<VueloViewModel>(respuesta);
    }

}
