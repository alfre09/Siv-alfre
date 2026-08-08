using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;
using System.Windows;

namespace Siv.Desktop.ViewModels;

public partial class AeropuertosViewModel : ViewModelBase
{
    private readonly IAeropuertoApiServicio _aeropuertoApiServicio;

    [ObservableProperty]
    private ObservableCollection<AeropuertoModelo> _aeropuertos = new();

    [ObservableProperty]
    private AeropuertoModelo? _aeropuertoSeleccionado;

    public AeropuertosViewModel(IAeropuertoApiServicio aeropuertoApiServicio)
    {
        _aeropuertoApiServicio = aeropuertoApiServicio;
        Task.Run(CargarDatosAsync);
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var aeropuertos = await _aeropuertoApiServicio.ObtenerTodosAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Aeropuertos = new ObservableCollection<AeropuertoModelo>(aeropuertos);
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar aeropuertos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
