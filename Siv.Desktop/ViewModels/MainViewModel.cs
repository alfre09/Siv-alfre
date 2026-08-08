using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Siv.Desktop.Servicios;

namespace Siv.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    public string RolActual => TokenManager.Rol;
    public bool EsAdmin => string.Equals(RolActual, "Admin", StringComparison.OrdinalIgnoreCase);
    public bool EsOperador => string.Equals(RolActual, "Operador", StringComparison.OrdinalIgnoreCase);
    public bool EsAuditor => string.Equals(RolActual, "Auditor", StringComparison.OrdinalIgnoreCase);
    public bool PuedeConsultarVuelos => EsAdmin || EsOperador || EsAuditor;
    public bool PuedeGestionarCambios => EsAdmin || EsOperador;
    public bool PuedeConsultarHistorial => EsAdmin || EsOperador || EsAuditor;
    public bool PuedeConsultarAuditoria => EsAdmin || EsAuditor;
    public bool PuedeConsultarNotificaciones => EsAdmin || EsAuditor;

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

    [RelayCommand]
    private void NavigateToHistorialEstados()
    {
        CurrentViewModel = _serviceProvider.GetRequiredService<HistorialEstadosViewModel>();
    }
}
