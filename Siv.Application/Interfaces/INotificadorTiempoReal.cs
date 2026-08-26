namespace Siv.Application.Interfaces;

public interface INotificadorTiempoReal
{
    Task<bool> EnviarNotificacionAsync(string usuario, string mensaje);
    Task<bool> EnviarNotificacionGeneralAsync(string mensaje);
}
