using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Siv.Domain.Repositorios;

namespace Siv.Persistence.Repositorios;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly SivDbContext _contexto;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaccionActiva;

    private IVueloRepositorio? _vuelos;
    private IAerolineaRepositorio? _aerolineas;
    private IAeropuertoRepositorio? _aeropuertos;
    private IEstadoVueloRepositorio? _estadosVuelo;
    private ISeguimientoRepositorio? _seguimientos;
    private ICambioOperativoRepositorio? _cambiosOperativos;
    private IHistorialEstadoVueloRepositorio? _historialEstados;
    private INotificacionRepositorio? _notificaciones;
    private IAuditoriaRepositorio? _auditorias;
    private IReservaRepositorio? _reservas;
    private IUsuarioRepositorio? _usuarios;

    public UnitOfWork(SivDbContext contexto, ILoggerFactory loggerFactory)
    {
        _contexto = contexto;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<UnitOfWork>();
    }

    public IVueloRepositorio Vuelos => _vuelos ??= new VueloRepositorio(_contexto, _loggerFactory.CreateLogger<VueloRepositorio>());
    public IAerolineaRepositorio Aerolineas => _aerolineas ??= new AerolineaRepositorio(_contexto, _loggerFactory.CreateLogger<AerolineaRepositorio>());
    public IAeropuertoRepositorio Aeropuertos => _aeropuertos ??= new AeropuertoRepositorio(_contexto, _loggerFactory.CreateLogger<AeropuertoRepositorio>());
    public IEstadoVueloRepositorio EstadosVuelo => _estadosVuelo ??= new EstadoVueloRepositorio(_contexto, _loggerFactory.CreateLogger<EstadoVueloRepositorio>());
    public ISeguimientoRepositorio Seguimientos => _seguimientos ??= new SeguimientoRepositorio(_contexto, _loggerFactory.CreateLogger<SeguimientoRepositorio>());
    public ICambioOperativoRepositorio CambiosOperativos => _cambiosOperativos ??= new CambioOperativoRepositorio(_contexto, _loggerFactory.CreateLogger<CambioOperativoRepositorio>());
    public IHistorialEstadoVueloRepositorio HistorialEstados => _historialEstados ??= new HistorialEstadoVueloRepositorio(_contexto, _loggerFactory.CreateLogger<HistorialEstadoVueloRepositorio>());
    public INotificacionRepositorio Notificaciones => _notificaciones ??= new NotificacionRepositorio(_contexto, _loggerFactory.CreateLogger<NotificacionRepositorio>());
    public IAuditoriaRepositorio Auditorias => _auditorias ??= new AuditoriaRepositorio(_contexto, _loggerFactory.CreateLogger<AuditoriaRepositorio>());
    public IReservaRepositorio Reservas => _reservas ??= new ReservaRepositorio(_contexto, _loggerFactory.CreateLogger<ReservaRepositorio>());
    public IUsuarioRepositorio Usuarios => _usuarios ??= new UsuarioRepositorio(_contexto, _loggerFactory.CreateLogger<UsuarioRepositorio>());

    public async Task<int> GuardarCambiosAsync()
    {
        _logger.LogInformation("Iniciando GuardarCambiosAsync");
        try
        {
            var resultado = await _contexto.SaveChangesAsync();
            _logger.LogInformation("GuardarCambiosAsync completado. Registros afectados: {Resultado}", resultado);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar cambios en la base de datos.");
            throw;
        }
    }

    public async Task EjecutarEnTransaccionAsync(Func<Task> operacion)
    {
        // El proveedor InMemory se usa únicamente en pruebas de integración y
        // no implementa transacciones relacionales.
        if (_transaccionActiva != null ||
            string.Equals(_contexto.Database.ProviderName, "Microsoft.EntityFrameworkCore.InMemory", StringComparison.Ordinal))
        {
            await operacion();
            return;
        }

        await using var transaccion = await _contexto.Database.BeginTransactionAsync();
        _transaccionActiva = transaccion;

        try
        {
            await operacion();
            await transaccion.CommitAsync();
            _logger.LogInformation("Transacción completada y confirmada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la ejecución de la transacción. Haciendo rollback.");
            await transaccion.RollbackAsync();
            throw;
        }
        finally
        {
            _transaccionActiva = null;
        }
    }

    public void Dispose()
    {
        _contexto.Dispose();
        GC.SuppressFinalize(this);
    }
}
