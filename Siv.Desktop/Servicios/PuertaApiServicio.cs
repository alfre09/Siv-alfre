using System.Net.Http;
using System.Net.Http.Json;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.Servicios;

public class PuertaApiServicio : ApiServicioBase, IPuertaApiServicio
{
    public PuertaApiServicio(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<List<PuertaModelo>> ObtenerTodasAsync()
    {
        var respuesta = await ClienteHttp.GetAsync("api/puertas");
        await AsegurarExitoAsync(respuesta);

        var puertas = await respuesta.Content.ReadFromJsonAsync<List<PuertaModelo>>();
        return puertas ?? new List<PuertaModelo>();
    }
}
