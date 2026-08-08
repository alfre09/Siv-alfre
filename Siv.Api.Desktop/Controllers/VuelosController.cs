using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VuelosController : ControllerBase
{
    private readonly IVueloServicio _vueloServicio;

    public VuelosController(IVueloServicio vueloServicio)
    {
        _vueloServicio = vueloServicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<VueloDto>>> ObtenerTodos()
    {
        var vuelos = await _vueloServicio.ObtenerTodosAsync();
        return Ok(vuelos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VueloDto>> ObtenerPorId(int id)
    {
        var rolUsuario = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var vuelo = await _vueloServicio.ObtenerPorIdAsync(id, rolUsuario);

        if (vuelo == null)
            return NotFound(new { mensaje = $"No existe un vuelo con id {id}." });

        return Ok(vuelo);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<VueloDto>> Crear([FromBody] CrearVueloDto dto)
    {
        var vueloCreado = await _vueloServicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = vueloCreado.VueloId }, vueloCreado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarVueloDto dto)
    {
        if (id != dto.VueloId)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del cuerpo de la solicitud." });

        var rolUsuario = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        await _vueloServicio.ActualizarAsync(dto, rolUsuario);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _vueloServicio.EliminarAsync(id);
        return NoContent();
    }
}
