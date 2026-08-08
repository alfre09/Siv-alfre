using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly INotificacionServicio _notificacionServicio;

    public NotificacionesController(INotificacionServicio notificacionServicio)
    {
        _notificacionServicio = notificacionServicio;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Auditor")]
    public async Task<ActionResult<List<NotificacionDto>>> ObtenerTodos()
    {
        var notificaciones = await _notificacionServicio.ObtenerTodosAsync();
        return Ok(notificaciones);
    }

    [HttpGet("usuario/{usuario}")]
    public async Task<ActionResult<List<NotificacionDto>>> ObtenerPorUsuario(string usuario)
    {
        var usuarioAutenticado = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuarioAutenticado))
            return Unauthorized();

        if (!User.IsInRole("Admin") && !User.IsInRole("Auditor") &&
            !usuarioAutenticado.Equals(usuario, StringComparison.OrdinalIgnoreCase))
            return Forbid();

        var notificaciones = await _notificacionServicio.ObtenerPorUsuarioAsync(usuario);
        return Ok(notificaciones);
    }

    [HttpGet("vuelo/{vueloId:int}")]
    [Authorize(Roles = "Admin,Auditor")]
    public async Task<ActionResult<List<NotificacionDto>>> ObtenerPorVuelo(int vueloId)
    {
        var notificaciones = await _notificacionServicio.ObtenerPorVueloAsync(vueloId);
        return Ok(notificaciones);
    }

    [HttpPatch("{id:int}/marcar-leida")]
    public async Task<IActionResult> MarcarComoLeida(int id)
    {
        await _notificacionServicio.MarcarComoLeidaAsync(id, User.Identity?.Name ?? string.Empty);
        return NoContent();
    }
}
