using Microsoft.AspNetCore.Mvc;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VuelosController : ControllerBase
{
    private readonly IVueloServicio _vueloServicio;

    public VuelosController(IVueloServicio vueloServicio)
    {
        _vueloServicio = vueloServicio;
    }

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<List<VueloDto>>> ObtenerTodos()
    {
        var rolUsuario = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var vuelos = await _vueloServicio.ObtenerTodosAsync(rolUsuario);
        return Ok(vuelos);
    }

    [HttpGet("disponibles")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<List<VueloDto>>> ObtenerDisponibles(
        [FromQuery] int? origenId,
        [FromQuery] int? destinoId,
        [FromQuery] DateTime? fecha)
    {
        var vuelos = await _vueloServicio.ObtenerDisponiblesAsync(origenId, destinoId, fecha);
        return Ok(vuelos);
    }

    [HttpGet("{id:int}")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<VueloDto>> ObtenerPorId(int id)
    {
        var rolUsuario = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var vuelo = await _vueloServicio.ObtenerPorIdAsync(id, rolUsuario);

        if (vuelo == null)
            return NotFound(new { mensaje = $"No existe un vuelo con id {id}." });

        return Ok(vuelo);
    }

}
