using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class EstadoVueloApiServicio : ApiServicioBase, IEstadoVueloApiServicio
{
    public EstadoVueloApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<List<EstadoVueloViewModel>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/estadosvuelo");
        return await LeerRespuestaExitosaAsync<List<EstadoVueloViewModel>>(respuesta);
    }
}
