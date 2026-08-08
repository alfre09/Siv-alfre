using Siv.Web.Interfaces;
using Siv.Web.Servicios;

namespace Siv.Web.Configuracion;

public static class DependenciasWeb
{
    public static IServiceCollection AgregarServiciosDeConsumoDeApi(
        this IServiceCollection servicios, IConfiguration configuracion)
    {
        var urlBaseApi = configuracion["ApiWeb:UrlBase"]
            ?? throw new InvalidOperationException(
                "No se ha configurado 'ApiWeb:UrlBase' en appsettings.json.");

        servicios.AddHttpContextAccessor();
        servicios.AddTransient<TokenHandler>();
        servicios.AddTransient<ApiResilienciaHandler>();

        void ConfigurarCliente(HttpClient cliente)
        {
            cliente.BaseAddress = new Uri(urlBaseApi);
            cliente.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        servicios.AddHttpClient<IVueloApiServicio, VueloApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        servicios.AddHttpClient<IReservaApiServicio, ReservaApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        servicios.AddHttpClient<IAeropuertoApiServicio, AeropuertoApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        servicios.AddHttpClient<ICambioOperativoApiServicio, CambioOperativoApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        servicios.AddHttpClient<ISeguimientoApiServicio, SeguimientoApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        servicios.AddHttpClient<INotificacionApiServicio, NotificacionApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        servicios.AddHttpClient<IAuditoriaApiServicio, AuditoriaApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();

        // HTTP Client extra para el AuthController que no necesita el token de JWT para enviar a login
        servicios.AddHttpClient<IAuthApiServicio, AuthApiServicio>(ConfigurarCliente)
            .AddHttpMessageHandler<ApiResilienciaHandler>();

        return servicios;
    }
}
