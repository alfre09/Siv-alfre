using System.Net.Http;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class EstadoVueloApiServicio : ApiServicioBase, IEstadoVueloApiServicio
{
    public EstadoVueloApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<EstadoVueloModelo>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/estadosvuelo");
        return await LeerRespuestaExitosaAsync<List<EstadoVueloModelo>>(respuesta);
    }
}
