using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "UsuarioRegistrado")]
public class ReservasController : ControllerBase
{
    private readonly IReservaServicio _reservaServicio;

    public ReservasController(IReservaServicio reservaServicio)
    {
        _reservaServicio = reservaServicio;
    }

    [HttpPost]
    public async Task<ActionResult<ReservaDto>> Crear([FromBody] CrearReservaDto dto)
    {
        var usuario = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuario))
            return Unauthorized();

        var reserva = await _reservaServicio.CrearAsync(dto.VueloId, usuario);
        return CreatedAtAction(nameof(ObtenerMisReservas), new { }, reserva);
    }

    [HttpGet("mis-reservas")]
    public async Task<ActionResult<List<ReservaDto>>> ObtenerMisReservas()
    {
        var usuario = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuario))
            return Unauthorized();

        return Ok(await _reservaServicio.ObtenerPorUsuarioAsync(usuario));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var usuario = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuario))
            return Unauthorized();

        await _reservaServicio.CancelarAsync(id, usuario);
        return NoContent();
    }
}
