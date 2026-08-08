namespace Siv.Application.Interfaces;

public interface INotificadorTiempoReal
{
    Task EnviarNotificacionAsync(string usuario, string mensaje);
    Task EnviarNotificacionGeneralAsync(string mensaje);
}
