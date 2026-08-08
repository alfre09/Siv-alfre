using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;
using System.Windows;
using Siv.Desktop.Servicios;

namespace Siv.Desktop.ViewModels;

public partial class NotificacionesViewModel : ViewModelBase
{
    private readonly INotificacionApiServicio _notificacionApiServicio;

    [ObservableProperty]
    private ObservableCollection<NotificacionModelo> _notificaciones = new();

    [ObservableProperty]
    private NotificacionModelo? _notificacionSeleccionada;

    public NotificacionesViewModel(INotificacionApiServicio notificacionApiServicio)
    {
        _notificacionApiServicio = notificacionApiServicio;
        _ = CargarDatosAsync();
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var notificaciones = await _notificacionApiServicio.ObtenerTodosAsync();
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Notificaciones = new ObservableCollection<NotificacionModelo>(notificaciones);
            });
        }
        catch (ExcepcionApi ex)
        {
            MessageBox.Show(ex.Message, "No se pudieron cargar las notificaciones", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar notificaciones: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    [RelayCommand]
    private async Task MarcarComoLeidaAsync(NotificacionModelo? notificacion)
    {
        if (notificacion == null || notificacion.Leida) return;
        
        try
        {
            await _notificacionApiServicio.MarcarComoLeidaAsync(notificacion.NotificacionId);
            notificacion.Leida = true;
            // Forzar actualización de la UI
            var index = Notificaciones.IndexOf(notificacion);
            Notificaciones[index] = notificacion;
        }
        catch (ExcepcionApi ex)
        {
            MessageBox.Show(ex.Message, "Error al marcar notificación", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al marcar notificación: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
