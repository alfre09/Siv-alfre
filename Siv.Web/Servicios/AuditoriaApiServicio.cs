using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class AuditoriaApiServicio : ApiServicioBase, IAuditoriaApiServicio
{
    public AuditoriaApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<List<AuditoriaViewModel>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/auditoria");
        return await LeerRespuestaExitosaAsync<List<AuditoriaViewModel>>(respuesta);
    }
}
