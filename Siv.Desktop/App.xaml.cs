using System.IO;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Servicios;
using Siv.Desktop.ViewModels;
using Siv.Desktop.Views;

namespace Siv.Desktop;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }
    public IConfiguration Configuration { get; private set; }

    public App()
    {
        // Setup configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();
        
        // Setup DI
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configuración
        services.AddSingleton(Configuration);

        services.AddTransient<TokenHandler>();
        services.AddTransient<ApiResilienciaHandler>();

        // API base URL
        var urlBaseApi = Configuration["ApiDesktop:UrlBase"] 
            ?? "https://localhost:7201"; // Fallback port

        void ConfigurarCliente(HttpClient cliente)
        {
            cliente.BaseAddress = new Uri(urlBaseApi);
            cliente.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        // HttpClientFactory + Services
        services.AddHttpClient<IVueloApiServicio, VueloApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<IAerolineaApiServicio, AerolineaApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<IAeropuertoApiServicio, AeropuertoApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<IEstadoVueloApiServicio, EstadoVueloApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<ICambioOperativoApiServicio, CambioOperativoApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<IPuertaApiServicio, PuertaApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<ISeguimientoApiServicio, SeguimientoApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<INotificacionApiServicio, NotificacionApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<IHistorialEstadoVueloApiServicio, HistorialEstadoVueloApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        services.AddHttpClient<IAuditoriaApiServicio, AuditoriaApiServicio>(ConfigurarCliente).AddHttpMessageHandler<TokenHandler>().AddHttpMessageHandler<ApiResilienciaHandler>();
        
        // HttpClient for Login (Auth)
        services.AddHttpClient("SivApi", ConfigurarCliente);

        // ViewModels
        services.AddTransient<VuelosViewModel>();
        services.AddTransient<CambiosOperativosViewModel>();
        services.AddTransient<AerolineasViewModel>();
        services.AddTransient<AeropuertosViewModel>();
        services.AddTransient<SeguimientosViewModel>();
        services.AddTransient<NotificacionesViewModel>();
        services.AddTransient<AuditoriasViewModel>();
        services.AddTransient<HistorialEstadosViewModel>();
        
        // Singleton para MainViewModel porque maneja la navegación principal
        services.AddSingleton<MainViewModel>();

        // Views
        services.AddTransient<MainWindow>();
        services.AddTransient<LoginWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }
    protected override void OnExit(ExitEventArgs e)
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        base.OnExit(e);
    }
}
