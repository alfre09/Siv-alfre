namespace Siv.Application.Interfaces;

public interface IEmailServicio
{
    Task EnviarAsync(string destinatario, string asunto, string contenido);
}
