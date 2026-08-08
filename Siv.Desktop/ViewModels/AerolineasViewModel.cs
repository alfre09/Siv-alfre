using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;
using System.Windows;

namespace Siv.Desktop.ViewModels;

public partial class AerolineasViewModel : ViewModelBase
{
    private readonly IAerolineaApiServicio _aerolineaApiServicio;

    [ObservableProperty]
    private ObservableCollection<AerolineaModelo> _aerolineas = new();

    [ObservableProperty]
    private AerolineaModelo? _aerolineaSeleccionada;

    public AerolineasViewModel(IAerolineaApiServicio aerolineaApiServicio)
    {
        _aerolineaApiServicio = aerolineaApiServicio;
        Task.Run(CargarDatosAsync);
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var aerolineas = await _aerolineaApiServicio.ObtenerTodosAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Aerolineas = new ObservableCollection<AerolineaModelo>(aerolineas);
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar aerolíneas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
