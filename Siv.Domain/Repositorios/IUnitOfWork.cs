namespace Siv.Domain.Repositorios;

public interface IUnitOfWork
{
    IVueloRepositorio Vuelos { get; }
    IAerolineaRepositorio Aerolineas { get; }
    IAeropuertoRepositorio Aeropuertos { get; }
    IEstadoVueloRepositorio EstadosVuelo { get; }
    ISeguimientoRepositorio Seguimientos { get; }
    ICambioOperativoRepositorio CambiosOperativos { get; }
    IHistorialEstadoVueloRepositorio HistorialEstados { get; }
    INotificacionRepositorio Notificaciones { get; }
    IAuditoriaRepositorio Auditorias { get; }
    IReservaRepositorio Reservas { get; }
    IUsuarioRepositorio Usuarios { get; }

    Task<int> GuardarCambiosAsync();
    Task EjecutarEnTransaccionAsync(Func<Task> operacion);
}
