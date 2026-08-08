using Microsoft.AspNetCore.Mvc;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "UsuarioRegistrado")]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionServicio _notificacionServicio;

    public NotificacionesController(INotificacionServicio notificacionServicio)
    {
        _notificacionServicio = notificacionServicio;
    }

    [HttpGet("usuario/{usuario}")]
    public async Task<ActionResult<List<NotificacionDto>>> ObtenerPorUsuario(string usuario)
    {
        var usuarioAutenticado = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuarioAutenticado))
            return Unauthorized();

        if (!usuarioAutenticado.Equals(usuario, StringComparison.OrdinalIgnoreCase))
            return Forbid();

        var notificaciones = await _notificacionServicio.ObtenerPorUsuarioAsync(usuario);
        return Ok(notificaciones);
    }

    [HttpPatch("{id:int}/marcar-leida")]
    public async Task<IActionResult> MarcarComoLeida(int id)
    {
        var usuario = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuario))
            return Unauthorized();

        await _notificacionServicio.MarcarComoLeidaAsync(id, usuario);
        return NoContent();
    }

    [HttpPost("difundir")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous] // Para permitir que Desktop envíe la notificación
    public async Task<IActionResult> Difundir([FromBody] DifundirRequest request)
    {
        // En un caso real usaríamos una clave compartida o un token para verificar que viene de la API Desktop
        // Aquí simplemente usamos el notificador para enviar la señal a los clientes de SignalR en la Web
        var notificador = HttpContext.RequestServices.GetRequiredService<INotificadorTiempoReal>();
        await notificador.EnviarNotificacionAsync(request.Usuario, request.Mensaje);
        return Ok();
    }
}

public class DifundirRequest
{
    public string Usuario { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}


