using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;
using System.Windows;

namespace Siv.Desktop.ViewModels;

public partial class AuditoriasViewModel : ViewModelBase
{
    private readonly IAuditoriaApiServicio _auditoriaApiServicio;

    [ObservableProperty]
    private ObservableCollection<AuditoriaModelo> _auditorias = new();

    public AuditoriasViewModel(IAuditoriaApiServicio auditoriaApiServicio)
    {
        _auditoriaApiServicio = auditoriaApiServicio;
        Task.Run(CargarDatosAsync);
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var auditorias = await _auditoriaApiServicio.ObtenerTodosAsync();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Auditorias = new ObservableCollection<AuditoriaModelo>(auditorias);
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar auditorías: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
