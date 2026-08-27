using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Siv.Application.Interfaces;

namespace Siv.Application.Servicios;

public sealed class SmtpEmailServicio : IEmailServicio
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailServicio> _logger;

    public SmtpEmailServicio(IConfiguration configuration, ILogger<SmtpEmailServicio> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task EnviarAsync(string destinatario, string asunto, string contenido)
    {
        if (!bool.TryParse(_configuration["Email:Enabled"], out var habilitado) || !habilitado)
        {
            _logger.LogDebug("El envío de correo está deshabilitado. No se envió el correo a {Destinatario}.", destinatario);
            return;
        }

        var host = _configuration["Email:Smtp:Host"];
        var from = _configuration["Email:Smtp:From"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            throw new InvalidOperationException("El correo está habilitado, pero falta configurar Email:Smtp:Host o Email:Smtp:From.");

        var port = int.TryParse(_configuration["Email:Smtp:Port"], out var configuredPort) ? configuredPort : 587;
        var enableSsl = !bool.TryParse(_configuration["Email:Smtp:EnableSsl"], out var configuredSsl) || configuredSsl;

        using var mensaje = new MailMessage(from, destinatario, asunto, contenido);
        using var cliente = new SmtpClient(host, port) { EnableSsl = enableSsl };

        var username = _configuration["Email:Smtp:Username"];
        var password = _configuration["Email:Smtp:Password"];
        if (!string.IsNullOrWhiteSpace(username))
            cliente.Credentials = new NetworkCredential(username, password ?? string.Empty);

        await cliente.SendMailAsync(mensaje);
        _logger.LogInformation("Correo de notificación enviado a {Destinatario}.", destinatario);
    }
}
