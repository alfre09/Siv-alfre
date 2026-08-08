using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Siv.Persistence;
using Xunit;

namespace Siv.Tests.Integracion;

public class SivApiWebIntegrationTests
{
    [Fact]
    public async Task Login_DebeRechazarClaveIncorrecta_YAceptarClaveValida()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();

        var incorrecto = await cliente.PostAsJsonAsync("api/auth/login", new
        {
            usuario = "admin",
            password = "incorrecta"
        });
        var correcto = await cliente.PostAsJsonAsync("api/auth/login", new
        {
            usuario = "admin",
            password = "Admin123!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, incorrecto.StatusCode);
        Assert.Equal(HttpStatusCode.OK, correcto.StatusCode);
        var respuesta = await correcto.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrWhiteSpace(respuesta.GetProperty("token").GetString()));
    }

    [Fact]
    public async Task Registro_DebeCrearUsuarioRegistradoYPermitirLogin()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();
        var nombre = $"cliente{Random.Shared.Next(1000, 9999)}";

        var registro = await cliente.PostAsJsonAsync("api/auth/registro", new
        {
            nombreUsuario = nombre,
            password = "Cliente123!"
        });
        var login = await cliente.PostAsJsonAsync("api/auth/login", new
        {
            usuario = nombre,
            password = "Cliente123!"
        });
        var duplicado = await cliente.PostAsJsonAsync("api/auth/registro", new
        {
            nombreUsuario = nombre,
            password = "Cliente123!"
        });

        Assert.Equal(HttpStatusCode.Created, registro.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicado.StatusCode);

        var respuestaLogin = await login.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("UsuarioRegistrado", respuestaLogin.GetProperty("rol").GetString());
    }

    [Fact]
    public async Task Historial_DebeDevolverCambiosDeVueloPublico()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();
        var vueloId = await factory.CrearVueloAsync(conCambio: true);

        var respuesta = await cliente.GetAsync($"api/cambiosoperativos/vuelo/{vueloId}");
        var contenido = await respuesta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains("Cambio de puerta", contenido);
    }

    [Fact]
    public async Task SeguimientoDuplicado_DebeSerIdempotente()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();
        var vueloId = await factory.CrearVueloAsync();
        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await ObtenerTokenAsync(cliente));

        var cuerpo = new { usuario = "admin", vueloId };
        var primeraRespuesta = await cliente.PostAsJsonAsync("api/seguimientos", cuerpo);
        var segundaRespuesta = await cliente.PostAsJsonAsync("api/seguimientos", cuerpo);

        Assert.Equal(HttpStatusCode.OK, primeraRespuesta.StatusCode);
        Assert.Equal(HttpStatusCode.OK, segundaRespuesta.StatusCode);

        using var alcance = factory.Services.CreateScope();
        var contexto = alcance.ServiceProvider.GetRequiredService<SivDbContext>();
        Assert.Equal(1, contexto.Seguimientos.Count(s => s.VueloId == vueloId && s.Usuario == "admin"));
    }

    [Fact]
    public async Task Seguimiento_DebeUsarElUsuarioDelTokenYNoElDelCuerpo()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();
        var vueloId = await factory.CrearVueloAsync();
        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await ObtenerTokenAsync(cliente, "cliente1", "Cliente123!"));

        var respuesta = await cliente.PostAsJsonAsync("api/seguimientos", new
        {
            usuario = "admin",
            vueloId
        });

        using var alcance = factory.Services.CreateScope();
        var contexto = alcance.ServiceProvider.GetRequiredService<SivDbContext>();

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.True(contexto.Seguimientos.Any(s => s.VueloId == vueloId && s.Usuario == "cliente1"));
        Assert.False(contexto.Seguimientos.Any(s => s.VueloId == vueloId && s.Usuario == "admin"));
    }

    [Fact]
    public async Task Roles_DebePermitirOperacionAlOperadorYAuditoriaAlAuditor()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();
        var vueloId = await factory.CrearVueloAsync();

        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await ObtenerTokenAsync(cliente, "operador1", "Operador123!"));
        var cambioOperador = await cliente.PostAsJsonAsync("api/cambiosoperativos/puerta", new
        {
            vueloId,
            nuevaPuerta = "C3",
            causa = "Cambio autorizado para operador"
        });
        var crearComoOperador = await cliente.PostAsJsonAsync("api/vuelos", new { numeroVuelo = "NO" });

        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await ObtenerTokenAsync(cliente, "auditor1", "Auditor123!"));
        var auditoria = await cliente.GetAsync("api/auditoria");
        var cambioComoAuditor = await cliente.PostAsJsonAsync("api/cambiosoperativos/puerta", new
        {
            vueloId,
            nuevaPuerta = "D4",
            causa = "No permitido"
        });

        Assert.Equal(HttpStatusCode.OK, cambioOperador.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, crearComoOperador.StatusCode);
        Assert.Equal(HttpStatusCode.OK, auditoria.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, cambioComoAuditor.StatusCode);
    }

    [Fact]
    public async Task CambioOperativo_DebeGenerarNotificacionAlSeguidor()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();
        var vueloId = await factory.CrearVueloAsync();
        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await ObtenerTokenAsync(cliente));

        var seguimiento = await cliente.PostAsJsonAsync("api/seguimientos", new
        {
            usuario = "admin",
            vueloId
        });
        var cambio = await cliente.PostAsJsonAsync("api/cambiosoperativos/puerta", new
        {
            vueloId,
            nuevaPuerta = "B2",
            causa = "Prueba de notificaciones"
        });
        var notificaciones = await cliente.GetAsync("api/notificaciones/usuario/admin");
        var contenido = await notificaciones.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, seguimiento.StatusCode);
        Assert.Equal(HttpStatusCode.OK, cambio.StatusCode);
        Assert.Equal(HttpStatusCode.OK, notificaciones.StatusCode);
        Assert.Contains("Prueba de notificaciones", contenido);
    }

    [Fact]
    public async Task VuelosDisponibles_DebeSerPublicoYMostrarSoloVuelosPublicosActivos()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();
        var vueloId = await factory.CrearVueloAsync();

        var respuesta = await cliente.GetAsync("api/vuelos/disponibles");
        var vuelos = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Contains(vuelos.EnumerateArray(), vuelo => vuelo.GetProperty("vueloId").GetInt32() == vueloId);
    }

    [Fact]
    public async Task Reserva_DebeCrearListarEvitarDuplicadoYCancelar()
    {
        using var factory = new SivApiWebFactory();
        using var cliente = factory.CreateClient();
        var vueloId = await factory.CrearVueloAsync();
        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await ObtenerTokenAsync(cliente, "cliente1", "Cliente123!"));

        var crear = await cliente.PostAsJsonAsync("api/reservas", new { vueloId });
        var reserva = await crear.Content.ReadFromJsonAsync<JsonElement>();
        var duplicada = await cliente.PostAsJsonAsync("api/reservas", new { vueloId });
        var listado = await cliente.GetFromJsonAsync<JsonElement>("api/reservas/mis-reservas");
        var reservaId = reserva.GetProperty("reservaId").GetInt32();
        var cancelar = await cliente.DeleteAsync($"api/reservas/{reservaId}");
        var listadoFinal = await cliente.GetFromJsonAsync<JsonElement>("api/reservas/mis-reservas");

        Assert.Equal(HttpStatusCode.Created, crear.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, duplicada.StatusCode);
        Assert.Contains(listado.EnumerateArray(), item => item.GetProperty("reservaId").GetInt32() == reservaId);
        Assert.Equal(HttpStatusCode.NoContent, cancelar.StatusCode);
        Assert.Equal("Cancelada", listadoFinal.EnumerateArray().Single().GetProperty("estado").GetString());
    }

    private static async Task<string> ObtenerTokenAsync(HttpClient cliente, string usuario = "admin", string password = "Admin123!")
    {
        var respuesta = await cliente.PostAsJsonAsync("api/auth/login", new
        {
            usuario,
            password
        });
        respuesta.EnsureSuccessStatusCode();
        var cuerpo = await respuesta.Content.ReadFromJsonAsync<JsonElement>();
        return cuerpo.GetProperty("token").GetString()!;
    }
}
