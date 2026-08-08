using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.ViewModels;

public partial class SeguimientosViewModel : ViewModelBase
{
    private readonly ISeguimientoApiServicio _seguimientoApiServicio;

    [ObservableProperty]
    private ObservableCollection<SeguimientoModelo> _seguimientos = new();

    [ObservableProperty]
    private SeguimientoModelo? _seguimientoSeleccionado;

    [ObservableProperty]
    private string _usuario = string.Empty;

    [ObservableProperty]
    private string _vueloIdTexto = string.Empty;

    public SeguimientosViewModel(ISeguimientoApiServicio seguimientoApiServicio)
    {
        _seguimientoApiServicio = seguimientoApiServicio;
        _ = CargarDatosAsync();
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var seguimientos = await _seguimientoApiServicio.ObtenerTodosAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Seguimientos = new ObservableCollection<SeguimientoModelo>(seguimientos);
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar seguimientos: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task CrearSeguimientoAsync()
    {
        if (string.IsNullOrWhiteSpace(Usuario))
        {
            MessageBox.Show("Indica el usuario interesado.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(VueloIdTexto, out var vueloId) || vueloId <= 0)
        {
            MessageBox.Show("Indica un ID de vuelo válido.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _seguimientoApiServicio.CrearAsync(new CrearSeguimientoModelo
            {
                Usuario = Usuario.Trim(),
                VueloId = vueloId
            });

            MessageBox.Show("Seguimiento creado correctamente.",
                "SIV", MessageBoxButton.OK, MessageBoxImage.Information);

            Usuario = string.Empty;
            VueloIdTexto = string.Empty;
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo crear el seguimiento: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task EliminarSeguimientoAsync()
    {
        if (SeguimientoSeleccionado == null)
        {
            MessageBox.Show("Selecciona un seguimiento.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _seguimientoApiServicio.EliminarAsync(SeguimientoSeleccionado.SeguimientoId);
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo eliminar el seguimiento: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
