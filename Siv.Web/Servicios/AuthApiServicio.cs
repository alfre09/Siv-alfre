using System.Net.Http.Json;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Servicios;

public class AuthApiServicio : ApiServicioBase, IAuthApiServicio
{
    public AuthApiServicio(HttpClient clienteHttp) : base(clienteHttp)
    {
    }

    public async Task<LoginResponseViewModel> LoginAsync(LoginViewModel modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/auth/login", modelo);
        return await LeerRespuestaExitosaAsync<LoginResponseViewModel>(respuesta);
    }

    public async Task RegistrarAsync(RegistroViewModel modelo)
    {
        var respuesta = await ClienteHttp.PostAsJsonAsync("api/auth/registro", new
        {
            nombreUsuario = modelo.NombreUsuario,
            password = modelo.Password
        });

        await AsegurarExitoAsync(respuesta);
    }
}
