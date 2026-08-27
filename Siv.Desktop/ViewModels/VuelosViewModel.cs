using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Siv.Desktop.Interfaces;
using Siv.Desktop.Modelos;
using System.Text.RegularExpressions;
using Siv.Desktop.Servicios;

namespace Siv.Desktop.ViewModels;

public partial class VuelosViewModel : ViewModelBase
{
    private readonly IVueloApiServicio _vueloApiServicio;
    private readonly IAerolineaApiServicio _aerolineaApiServicio;
    private readonly IAeropuertoApiServicio _aeropuertoApiServicio;
    private readonly IPuertaApiServicio _puertaApiServicio;
    private List<PuertaModelo> _todasLasPuertas = new();

    [ObservableProperty] private ObservableCollection<VueloModelo> _vuelos = new();
    [ObservableProperty] private VueloModelo? _vueloSeleccionado;
    [ObservableProperty] private ObservableCollection<AerolineaModelo> _aerolineas = new();
    [ObservableProperty] private ObservableCollection<AeropuertoModelo> _aeropuertos = new();
    [ObservableProperty] private ObservableCollection<PuertaModelo> _puertasDisponibles = new();

    [ObservableProperty] private string _numeroVueloNuevo = string.Empty;
    [ObservableProperty] private AerolineaModelo? _aerolineaNueva;
    [ObservableProperty] private AeropuertoModelo? _aeropuertoOrigenNuevo;
    [ObservableProperty] private AeropuertoModelo? _aeropuertoDestinoNuevo;
    [ObservableProperty] private string _horarioProgramadoNuevo = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm");
    [ObservableProperty] private PuertaModelo? _puertaNueva;
    [ObservableProperty] private string _nivelVisibilidadNuevo = "Publico";

    public List<string> NivelesVisibilidad { get; } = new() { "Publico", "Interno", "Restringido" };
    public bool EsAdmin => string.Equals(TokenManager.Rol, "Admin", StringComparison.OrdinalIgnoreCase);

    public VuelosViewModel(
        IVueloApiServicio vueloApiServicio,
        IAerolineaApiServicio aerolineaApiServicio,
        IAeropuertoApiServicio aeropuertoApiServicio,
        IPuertaApiServicio puertaApiServicio)
    {
        _vueloApiServicio = vueloApiServicio;
        _aerolineaApiServicio = aerolineaApiServicio;
        _aeropuertoApiServicio = aeropuertoApiServicio;
        _puertaApiServicio = puertaApiServicio;

        _ = CargarDatosAsync();
    }

    partial void OnAeropuertoOrigenNuevoChanged(AeropuertoModelo? value) => ActualizarPuertasDisponibles();
    partial void OnHorarioProgramadoNuevoChanged(string value) => ActualizarPuertasDisponibles();

    [RelayCommand]
    private async Task CargarDatosAsync()
    {
        try
        {
            var vuelos = await _vueloApiServicio.ObtenerTodosAsync();
            var aerolineas = await _aerolineaApiServicio.ObtenerTodosAsync();
            var aeropuertos = await _aeropuertoApiServicio.ObtenerTodosAsync();
            var puertas = await _puertaApiServicio.ObtenerTodasAsync();

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Vuelos = new ObservableCollection<VueloModelo>(vuelos);
                Aerolineas = new ObservableCollection<AerolineaModelo>(aerolineas);
                Aeropuertos = new ObservableCollection<AeropuertoModelo>(aeropuertos);
                _todasLasPuertas = puertas;
                ActualizarPuertasDisponibles();
            });
        }
        catch (ExcepcionApi ex)
        {
            MessageBox.Show(ex.Message, "No se pudieron cargar los vuelos", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Vuelos", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task CrearVueloAsync()
    {
        if (string.IsNullOrWhiteSpace(NumeroVueloNuevo) || NumeroVueloNuevo.Length < 3 || NumeroVueloNuevo.Length > 10)
        {
            MostrarValidacion("El número de vuelo debe tener entre 3 y 10 caracteres.");
            return;
        }

        if (!Regex.IsMatch(NumeroVueloNuevo, "^[a-zA-Z0-9]+$"))
        {
            MostrarValidacion("El número de vuelo solo puede contener letras y números, sin espacios.");
            return;
        }

        if (AerolineaNueva is null || AeropuertoOrigenNuevo is null || AeropuertoDestinoNuevo is null)
        {
            MostrarValidacion("Selecciona la aerolínea, el origen y el destino.");
            return;
        }

        if (AeropuertoOrigenNuevo.AeropuertoId == AeropuertoDestinoNuevo.AeropuertoId)
        {
            MostrarValidacion("El aeropuerto de origen y el destino deben ser diferentes.");
            return;
        }

        if (!DateTime.TryParse(HorarioProgramadoNuevo, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var horario))
        {
            MostrarValidacion("El horario no es válido. Usa el formato dd/MM/yyyy HH:mm.");
            return;
        }

        if (horario <= DateTime.Now)
        {
            MostrarValidacion("El horario programado debe ser en el futuro.");
            return;
        }

        if (PuertaNueva is not null && !PuertasDisponibles.Contains(PuertaNueva))
        {
            MostrarValidacion("La puerta seleccionada no está disponible para ese aeropuerto y horario.");
            return;
        }

        try
        {
            await _vueloApiServicio.CrearAsync(new CrearVueloModelo
            {
                NumeroVuelo = NumeroVueloNuevo.Trim(),
                AerolineaId = AerolineaNueva.AerolineaId,
                AeropuertoOrigenId = AeropuertoOrigenNuevo.AeropuertoId,
                AeropuertoDestinoId = AeropuertoDestinoNuevo.AeropuertoId,
                HorarioProgramado = horario,
                Puerta = PuertaNueva?.Codigo,
                NivelVisibilidad = NivelVisibilidadNuevo
            });

            LimpiarFormulario();
            await CargarDatosAsync();
            MessageBox.Show("El vuelo se creó correctamente.", "Vuelos", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (ExcepcionApi ex)
        {
            MessageBox.Show(ex.Message, "No se pudo crear el vuelo", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Vuelos", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LimpiarFormulario()
    {
        NumeroVueloNuevo = string.Empty;
        AerolineaNueva = null;
        AeropuertoOrigenNuevo = null;
        AeropuertoDestinoNuevo = null;
        HorarioProgramadoNuevo = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy HH:mm");
        PuertaNueva = null;
        NivelVisibilidadNuevo = "Publico";
    }

    private void ActualizarPuertasDisponibles()
    {
        if (AeropuertoOrigenNuevo is null)
        {
            PuertasDisponibles = new ObservableCollection<PuertaModelo>(_todasLasPuertas);
            return;
        }

        var horarioValido = DateTime.TryParse(
            HorarioProgramadoNuevo,
            CultureInfo.CurrentCulture,
            DateTimeStyles.AllowWhiteSpaces,
            out var horario);

        var ocupadas = horarioValido
            ? Vuelos.Where(v =>
                    v.AeropuertoOrigenId == AeropuertoOrigenNuevo.AeropuertoId &&
                    !string.IsNullOrWhiteSpace(v.Puerta) &&
                    Math.Abs((v.HorarioProgramado - horario).TotalMinutes) < 120 &&
                    !string.Equals(v.EstadoVueloNombre, "Cancelado", StringComparison.OrdinalIgnoreCase))
                .Select(v => v.Puerta!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
            : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var disponibles = _todasLasPuertas
            .Where(p => p.AeropuertoId == AeropuertoOrigenNuevo.AeropuertoId &&
                        (!ocupadas.Contains(p.Codigo) ||
                         (PuertaNueva is not null && p.Id == PuertaNueva.Id)))
            .OrderBy(p => p.Codigo, StringComparer.OrdinalIgnoreCase)
            .ToList();

        PuertasDisponibles = new ObservableCollection<PuertaModelo>(disponibles);
        if (PuertaNueva is not null && !disponibles.Any(p => p.Id == PuertaNueva.Id))
            PuertaNueva = null;
    }

    private static void MostrarValidacion(string mensaje) =>
        MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
}
