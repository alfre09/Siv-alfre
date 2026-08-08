using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Siv.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        // Set default view
        NavigateToVuelos();
    }

    [RelayCommand]
    private void NavigateToVuelos()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<VuelosViewModel>();
    }

    [RelayCommand]
    private void NavigateToAerolineas()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<AerolineasViewModel>();
    }

    [RelayCommand]
    private void NavigateToAeropuertos()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<AeropuertosViewModel>();
    }

    [RelayCommand]
    private void NavigateToCambiosOperativos()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<CambiosOperativosViewModel>();
    }

    [RelayCommand]
    private void NavigateToSeguimientos()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<SeguimientosViewModel>();
    }

    [RelayCommand]
    private void NavigateToNotificaciones()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<NotificacionesViewModel>();
    }

    [RelayCommand]
    private void NavigateToAuditorias()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<AuditoriasViewModel>();
    }
}
