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
}


