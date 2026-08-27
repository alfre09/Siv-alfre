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
    private readonly IVueloApiServicio _vueloApiServicio;
    private readonly IPuertaApiServicio _puertaApiServicio;
    private List<PuertaModelo> _todasLasPuertas = new();

    [ObservableProperty]
    private ObservableCollection<CambioOperativoModelo> _cambios = new();

    [ObservableProperty]
    private ObservableCollection<EstadoVueloModelo> _estados = new();

    [ObservableProperty]
    private CambioOperativoModelo? _cambioSeleccionado;

    [ObservableProperty]
    private ObservableCollection<VueloModelo> _vuelos = new();

    [ObservableProperty]
    private ObservableCollection<PuertaModelo> _puertas = new();

    [ObservableProperty]
    private VueloModelo? _vueloSeleccionado;

    partial void OnVueloSeleccionadoChanged(VueloModelo? value)
    {
        if (value != null)
        {
            NuevoHorarioTexto = value.HorarioProgramado.ToString("dd/MM/yyyy HH:mm");
            ActualizarPuertasDisponibles(value);
            
            if (!string.IsNullOrWhiteSpace(value.Puerta))
            {
                PuertaSeleccionada = Puertas.FirstOrDefault(p => p.Codigo == value.Puerta);
            }
            else
            {
                PuertaSeleccionada = null;
            }

            if (value.EstadoVueloId > 0)
            {
                EstadoSeleccionado = Estados.FirstOrDefault(e => e.EstadoVueloId == value.EstadoVueloId);
            }
            else
            {
                EstadoSeleccionado = null;
            }
        }
        else
        {
            LimpiarFormulario(false);
        }
    }

    [ObservableProperty]
    private string _tipoCambioSeleccionado = "Retraso";

    [ObservableProperty]
    private string _nuevoHorarioTexto = string.Empty;

    [ObservableProperty]
    private PuertaModelo? _puertaSeleccionada;

    [ObservableProperty]
    private EstadoVueloModelo? _estadoSeleccionado;

    [ObservableProperty]
    private string _causa = string.Empty;

    public List<string> TiposCambio { get; } =
        new() { "Retraso", "Adelanto", "CambioPuerta", "CambioEstado", "Cancelacion" };

    public CambiosOperativosViewModel(
        ICambioOperativoApiServicio cambioOperativoApiServicio,
        IEstadoVueloApiServicio estadoVueloApiServicio,
        IVueloApiServicio vueloApiServicio,
        IPuertaApiServicio puertaApiServicio)
    {
        _cambioOperativoApiServicio = cambioOperativoApiServicio;
        _estadoVueloApiServicio = estadoVueloApiServicio;
        _vueloApiServicio = vueloApiServicio;
        _puertaApiServicio = puertaApiServicio;
        _ = CargarDatosAsync();
    }

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var cambios = await _cambioOperativoApiServicio.ObtenerTodosAsync();
            var estados = await _estadoVueloApiServicio.ObtenerTodosAsync();
            var vuelos = await _vueloApiServicio.ObtenerTodosAsync();
            var puertas = await _puertaApiServicio.ObtenerTodasAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Cambios = new ObservableCollection<CambioOperativoModelo>(cambios);
                Estados = new ObservableCollection<EstadoVueloModelo>(estados);
                Vuelos = new ObservableCollection<VueloModelo>(vuelos);
                _todasLasPuertas = puertas;
                ActualizarPuertasDisponibles(VueloSeleccionado);
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
        if (VueloSeleccionado == null)
        {
            MessageBox.Show("Selecciona un vuelo válido.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        var vueloId = VueloSeleccionado.VueloId;

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
                    if (PuertaSeleccionada == null)
                    {
                        MessageBox.Show("Selecciona la nueva puerta.", "Validación",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    await _cambioOperativoApiServicio.RegistrarCambioPuertaAsync(
                        new RegistrarCambioPuertaModelo
                        {
                            VueloId = vueloId,
                            NuevaPuerta = PuertaSeleccionada.Codigo,
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

    private void LimpiarFormulario(bool limpiarVuelo = true)
    {
        if (limpiarVuelo)
            VueloSeleccionado = null;
            
        NuevoHorarioTexto = string.Empty;
        PuertaSeleccionada = null;
        Causa = string.Empty;
        EstadoSeleccionado = null;
    }

    private void ActualizarPuertasDisponibles(VueloModelo? vuelo)
    {
        if (vuelo is null)
        {
            Puertas = new ObservableCollection<PuertaModelo>(_todasLasPuertas);
            return;
        }

        var ocupadas = Vuelos
            .Where(v => v.VueloId != vuelo.VueloId &&
                        v.AeropuertoOrigenId == vuelo.AeropuertoOrigenId &&
                        !string.IsNullOrWhiteSpace(v.Puerta) &&
                        Math.Abs((v.HorarioProgramado - vuelo.HorarioProgramado).TotalMinutes) < 120 &&
                        !string.Equals(v.EstadoVueloNombre, "Cancelado", StringComparison.OrdinalIgnoreCase))
            .Select(v => v.Puerta!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Puertas = new ObservableCollection<PuertaModelo>(
            _todasLasPuertas
                .Where(p => p.AeropuertoId == vuelo.AeropuertoOrigenId &&
                            (!ocupadas.Contains(p.Codigo) ||
                             string.Equals(p.Codigo, vuelo.Puerta, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(p => p.Codigo, StringComparer.OrdinalIgnoreCase));
    }
}
