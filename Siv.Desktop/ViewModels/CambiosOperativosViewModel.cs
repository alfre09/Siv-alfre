using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;

namespace Siv.Desktop.ViewModels;

public partial class CambiosOperativosViewModel : ViewModelBase
{
    private readonly ICambioOperativoApiServicio _cambioOperativoApiServicio;
    private readonly IEstadoVueloApiServicio _estadoVueloApiServicio;

    [ObservableProperty]
    private ObservableCollection<CambioOperativoModelo> _cambios = new();

    [ObservableProperty]
    private ObservableCollection<EstadoVueloModelo> _estados = new();

    [ObservableProperty]
    private CambioOperativoModelo? _cambioSeleccionado;

    [ObservableProperty]
    private string _vueloIdTexto = string.Empty;

    [ObservableProperty]
    private string _tipoCambioSeleccionado = "Retraso";

    [ObservableProperty]
    private string _nuevoHorarioTexto = string.Empty;

    [ObservableProperty]
    private string _nuevaPuerta = string.Empty;

    [ObservableProperty]
    private EstadoVueloModelo? _estadoSeleccionado;

    [ObservableProperty]
    private string _causa = string.Empty;

    public List<string> TiposCambio { get; } =
        new() { "Retraso", "Adelanto", "CambioPuerta", "CambioEstado", "Cancelacion" };

    public CambiosOperativosViewModel(
        ICambioOperativoApiServicio cambioOperativoApiServicio,
        IEstadoVueloApiServicio estadoVueloApiServicio)
    {
        _cambioOperativoApiServicio = cambioOperativoApiServicio;
        _estadoVueloApiServicio = estadoVueloApiServicio;
        _ = CargarDatosAsync();
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var cambios = await _cambioOperativoApiServicio.ObtenerTodosAsync();
            var estados = await _estadoVueloApiServicio.ObtenerTodosAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Cambios = new ObservableCollection<CambioOperativoModelo>(cambios);
                Estados = new ObservableCollection<EstadoVueloModelo>(estados);
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar cambios operativos: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task RegistrarCambioAsync()
    {
        if (!int.TryParse(VueloIdTexto, out var vueloId) || vueloId <= 0)
        {
            MessageBox.Show("Indica un ID de vuelo válido.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Causa))
        {
            MessageBox.Show("La causa del cambio es obligatoria.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            switch (TipoCambioSeleccionado)
            {
                case "Retraso":
                case "Adelanto":
                    if (!DateTime.TryParse(NuevoHorarioTexto, CultureInfo.CurrentCulture,
                            DateTimeStyles.None, out var nuevoHorario))
                    {
                        MessageBox.Show("Indica un horario válido.", "Validación",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    await _cambioOperativoApiServicio.RegistrarRetrasoOAdelantoAsync(
                        new RegistrarRetrasoOAdelantoModelo
                        {
                            VueloId = vueloId,
                            EsAdelanto = TipoCambioSeleccionado == "Adelanto",
                            NuevoHorario = nuevoHorario,
                            Causa = Causa.Trim()
                        });
                    break;

                case "CambioPuerta":
                    if (string.IsNullOrWhiteSpace(NuevaPuerta))
                    {
                        MessageBox.Show("Indica la nueva puerta.", "Validación",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    await _cambioOperativoApiServicio.RegistrarCambioPuertaAsync(
                        new RegistrarCambioPuertaModelo
                        {
                            VueloId = vueloId,
                            NuevaPuerta = NuevaPuerta.Trim(),
                            Causa = Causa.Trim()
                        });
                    break;

                case "CambioEstado":
                    if (EstadoSeleccionado == null)
                    {
                        MessageBox.Show("Selecciona el nuevo estado.", "Validación",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    await _cambioOperativoApiServicio.RegistrarCambioEstadoAsync(
                        new RegistrarCambioEstadoModelo
                        {
                            VueloId = vueloId,
                            NuevoEstadoVueloId = EstadoSeleccionado.EstadoVueloId,
                            Causa = Causa.Trim()
                        });
                    break;

                case "Cancelacion":
                    await _cambioOperativoApiServicio.RegistrarCancelacionAsync(
                        new RegistrarCancelacionModelo
                        {
                            VueloId = vueloId,
                            Causa = Causa.Trim()
                        });
                    break;
            }

            MessageBox.Show(
                "Cambio registrado correctamente. Se actualizó el historial y se notificó a los usuarios que siguen el vuelo.",
                "SIV", MessageBoxButton.OK, MessageBoxImage.Information);

            LimpiarFormulario();
            await CargarDatosAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo registrar el cambio: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LimpiarFormulario()
    {
        VueloIdTexto = string.Empty;
        NuevoHorarioTexto = string.Empty;
        NuevaPuerta = string.Empty;
        Causa = string.Empty;
        EstadoSeleccionado = null;
    }
}
