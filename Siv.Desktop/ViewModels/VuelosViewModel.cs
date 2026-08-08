using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;
using System.Windows;

namespace Siv.Desktop.ViewModels;

public partial class VuelosViewModel : ViewModelBase
{
    private readonly IVueloApiServicio _vueloApiServicio;
    private readonly IAerolineaApiServicio _aerolineaApiServicio;
    private readonly IAeropuertoApiServicio _aeropuertoApiServicio;

    [ObservableProperty]
    private ObservableCollection<VueloModelo> _vuelos = new();

    [ObservableProperty]
    private VueloModelo? _vueloSeleccionado;

    [ObservableProperty]
    private ObservableCollection<AerolineaModelo> _aerolineas = new();

    [ObservableProperty]
    private ObservableCollection<AeropuertoModelo> _aeropuertos = new();

    public VuelosViewModel(
        IVueloApiServicio vueloApiServicio,
        IAerolineaApiServicio aerolineaApiServicio,
        IAeropuertoApiServicio aeropuertoApiServicio)
    {
        _vueloApiServicio = vueloApiServicio;
        _aerolineaApiServicio = aerolineaApiServicio;
        _aeropuertoApiServicio = aeropuertoApiServicio;
        
        Task.Run(CargarDatosAsync);
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var vuelos = await _vueloApiServicio.ObtenerTodosAsync();
            var aerolineas = await _aerolineaApiServicio.ObtenerTodosAsync();
            var aeropuertos = await _aeropuertoApiServicio.ObtenerTodosAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Vuelos = new ObservableCollection<VueloModelo>(vuelos);
                Aerolineas = new ObservableCollection<AerolineaModelo>(aerolineas);
                Aeropuertos = new ObservableCollection<AeropuertoModelo>(aeropuertos);
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar vuelos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
