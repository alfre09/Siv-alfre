using System.Net.Http;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class HistorialEstadoVueloApiServicio : ApiServicioBase, IHistorialEstadoVueloApiServicio
{
    public HistorialEstadoVueloApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<HistorialEstadoVueloModelo>> ObtenerPorVueloAsync(int vueloId)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/historialestados/vuelo/{vueloId}");
        return await LeerRespuestaExitosaAsync<List<HistorialEstadoVueloModelo>>(respuesta);
    }
}
