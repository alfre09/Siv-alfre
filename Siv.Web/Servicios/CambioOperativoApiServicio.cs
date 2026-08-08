using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class CambioOperativoApiServicio : ApiServicioBase, ICambioOperativoApiServicio
{
    public CambioOperativoApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<List<CambioOperativoViewModel>> ObtenerPorVueloAsync(int vueloId)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/cambiosoperativos/vuelo/{vueloId}");
        return await LeerRespuestaExitosaAsync<List<CambioOperativoViewModel>>(respuesta);
    }
}
