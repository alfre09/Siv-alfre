using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeguimientosController : ControllerBase
{
    private readonly ISeguimientoServicio _seguimientoServicio;

    public SeguimientosController(ISeguimientoServicio seguimientoServicio)
    {
        _seguimientoServicio = seguimientoServicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<SeguimientoDto>>> ObtenerTodos()
    {
        var seguimientos = await _seguimientoServicio.ObtenerTodosAsync();
        return Ok(seguimientos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SeguimientoDto>> ObtenerPorId(int id)
    {
        var seguimiento = await _seguimientoServicio.ObtenerPorIdAsync(id);

        if (seguimiento == null)
            return NotFound(new { mensaje = $"No existe un seguimiento con id {id}." });

        return Ok(seguimiento);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SeguimientoDto>> Crear([FromBody] CrearSeguimientoDto dto)
    {
        var seguimientoCreado = await _seguimientoServicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = seguimientoCreado.SeguimientoId }, seguimientoCreado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarSeguimientoDto dto)
    {
        if (id != dto.SeguimientoId)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del cuerpo de la solicitud." });

        await _seguimientoServicio.ActualizarAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _seguimientoServicio.EliminarAsync(id);
        return NoContent();
    }
}
