using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;
using Siv.Desktop.Servicios;

namespace Siv.Desktop.ViewModels;

public partial class VuelosViewModel : ViewModelBase
{
    private readonly IVueloApiServicio _vueloApiServicio;
    private readonly IAerolineaApiServicio _aerolineaApiServicio;
    private readonly IAeropuertoApiServicio _aeropuertoApiServicio;

    [ObservableProperty] private ObservableCollection<VueloModelo> _vuelos = new();
    [ObservableProperty] private VueloModelo? _vueloSeleccionado;
    [ObservableProperty] private ObservableCollection<AerolineaModelo> _aerolineas = new();
    [ObservableProperty] private ObservableCollection<AeropuertoModelo> _aeropuertos = new();

    [ObservableProperty] private string _numeroVueloNuevo = string.Empty;
    [ObservableProperty] private AerolineaModelo? _aerolineaNueva;
    [ObservableProperty] private AeropuertoModelo? _aeropuertoOrigenNuevo;
    [ObservableProperty] private AeropuertoModelo? _aeropuertoDestinoNuevo;
    [ObservableProperty] private string _horarioProgramadoNuevo = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm");
    [ObservableProperty] private string _puertaNueva = string.Empty;
    [ObservableProperty] private string _nivelVisibilidadNuevo = "Publico";

    public List<string> NivelesVisibilidad { get; } = new() { "Publico", "Interno", "Restringido" };

    public VuelosViewModel(
        IVueloApiServicio vueloApiServicio,
        IAerolineaApiServicio aerolineaApiServicio,
        IAeropuertoApiServicio aeropuertoApiServicio)
    {
        _vueloApiServicio = vueloApiServicio;
        _aerolineaApiServicio = aerolineaApiServicio;
        _aeropuertoApiServicio = aeropuertoApiServicio;

        _ = CargarDatosAsync();
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var vuelos = await _vueloApiServicio.ObtenerTodosAsync();
            var aerolineas = await _aerolineaApiServicio.ObtenerTodosAsync();
            var aeropuertos = await _aeropuertoApiServicio.ObtenerTodosAsync();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Vuelos = new ObservableCollection<VueloModelo>(vuelos);
                Aerolineas = new ObservableCollection<AerolineaModelo>(aerolineas);
                Aeropuertos = new ObservableCollection<AeropuertoModelo>(aeropuertos);
            });
        }
        catch (ExcepcionApi ex)
        {
            MessageBox.Show(ex.Message, "No se pudieron cargar los vuelos", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Vuelos", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task CrearVueloAsync()
    {
        if (string.IsNullOrWhiteSpace(NumeroVueloNuevo))
        {
            MostrarValidacion("Indica el número de vuelo.");
            return;
        }

        if (AerolineaNueva is null || AeropuertoOrigenNuevo is null || AeropuertoDestinoNuevo is null)
        {
            MostrarValidacion("Selecciona la aerolínea, el origen y el destino.");
            return;
        }

        if (AeropuertoOrigenNuevo.AeropuertoId == AeropuertoDestinoNuevo.AeropuertoId)
        {
            MostrarValidacion("El aeropuerto de origen y el destino deben ser diferentes.");
            return;
        }

        if (!DateTime.TryParse(HorarioProgramadoNuevo, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var horario))
        {
            MostrarValidacion("El horario no es válido. Usa el formato dd/MM/yyyy HH:mm.");
            return;
        }

        try
        {
            await _vueloApiServicio.CrearAsync(new CrearVueloModelo
            {
                NumeroVuelo = NumeroVueloNuevo.Trim(),
                AerolineaId = AerolineaNueva.AerolineaId,
                AeropuertoOrigenId = AeropuertoOrigenNuevo.AeropuertoId,
                AeropuertoDestinoId = AeropuertoDestinoNuevo.AeropuertoId,
                HorarioProgramado = horario,
                Puerta = string.IsNullOrWhiteSpace(PuertaNueva) ? null : PuertaNueva.Trim(),
                NivelVisibilidad = NivelVisibilidadNuevo
            });

            LimpiarFormulario();
            await CargarDatosAsync();
            MessageBox.Show("El vuelo se creó correctamente.", "Vuelos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (ExcepcionApi ex)
        {
            MessageBox.Show(ex.Message, "No se pudo crear el vuelo", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Vuelos", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LimpiarFormulario()
    {
        NumeroVueloNuevo = string.Empty;
        AerolineaNueva = null;
        AeropuertoOrigenNuevo = null;
        AeropuertoDestinoNuevo = null;
        HorarioProgramadoNuevo = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm");
        PuertaNueva = string.Empty;
        NivelVisibilidadNuevo = "Publico";
    }

    private static void MostrarValidacion(string mensaje) =>
        MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
}
