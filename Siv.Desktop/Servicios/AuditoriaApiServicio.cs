using System.Net.Http;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class AuditoriaApiServicio : ApiServicioBase, IAuditoriaApiServicio
{
    public AuditoriaApiServicio(HttpClient clienteHttp) : base(clienteHttp) { }

    public async Task<List<AuditoriaModelo>> ObtenerTodosAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/auditorias");
        return await LeerRespuestaExitosaAsync<List<AuditoriaModelo>>(respuesta);
    }

    public async Task<List<AuditoriaModelo>> ObtenerPorTablaAsync(string tabla)
    {
        var respuesta = await ClienteHttp.GetAsync($"api/auditorias/tabla/{Uri.EscapeDataString(tabla)}");
        return await LeerRespuestaExitosaAsync<List<AuditoriaModelo>>(respuesta);
    }
}
