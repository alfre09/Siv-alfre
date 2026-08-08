using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.ViewModels;

public partial class HistorialEstadosViewModel : ViewModelBase
{
    private readonly IHistorialEstadoVueloApiServicio _historialApiServicio;
    [ObservableProperty] private string _vueloIdTexto = string.Empty;
    [ObservableProperty] private ObservableCollection<HistorialEstadoVueloModelo> _historial = new();

    public HistorialEstadosViewModel(IHistorialEstadoVueloApiServicio historialApiServicio) => _historialApiServicio = historialApiServicio;

    [RelayCommand]
    private async Task ConsultarAsync()
    {
        if (!int.TryParse(VueloIdTexto, out var vueloId) || vueloId <= 0)
        {
            MessageBox.Show("Indica un ID de vuelo válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        try { Historial = new ObservableCollection<HistorialEstadoVueloModelo>(await _historialApiServicio.ObtenerPorVueloAsync(vueloId)); }
        catch (Exception ex) { MessageBox.Show($"No se pudo consultar el historial: {ex.Message}", "Historial", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}
