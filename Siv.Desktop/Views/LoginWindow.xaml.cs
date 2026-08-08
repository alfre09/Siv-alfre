using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Siv.Desktop.Servicios;

namespace Siv.Desktop.Views;

public partial class LoginWindow : Window
{
    private readonly IServiceProvider _serviceProvider;
    private readonly HttpClient _httpClient;

    public LoginWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
        _httpClient = httpClientFactory.CreateClient("SivApi");
    }

    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        var usuario = TxtUsuario.Text;
        var password = TxtPassword.Password;

        if (string.IsNullOrWhiteSpace(usuario))
        {
            TxtMensajeError.Text = "El usuario es obligatorio.";
            TxtMensajeError.Visibility = Visibility.Visible;
            return;
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Usuario = usuario, Password = password });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    // Guardar el token en el TokenManager global
                    TokenManager.Token = result.Token;
                    
                    // Abrir MainWindow
                    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    mainWindow.Show();
                    
                    // Cerrar LoginWindow
                    this.Close();
                    return;
                }
            }
            
            TxtMensajeError.Text = "Credenciales inválidas.";
            TxtMensajeError.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            TxtMensajeError.Text = "Error al conectar con el servidor.";
            TxtMensajeError.Visibility = Visibility.Visible;
        }
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
