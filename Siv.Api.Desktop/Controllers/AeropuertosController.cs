using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AeropuertosController : ControllerBase
{
    private readonly IAeropuertoServicio _aeropuertoServicio;

    public AeropuertosController(IAeropuertoServicio aeropuertoServicio)
    {
        _aeropuertoServicio = aeropuertoServicio;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador,Auditor")]
    public async Task<ActionResult<List<AeropuertoDto>>> ObtenerTodos()
    {
        var aeropuertos = await _aeropuertoServicio.ObtenerTodosAsync();
        return Ok(aeropuertos);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Operador,Auditor")]
    public async Task<ActionResult<AeropuertoDto>> ObtenerPorId(int id)
    {
        var aeropuerto = await _aeropuertoServicio.ObtenerPorIdAsync(id);

        if (aeropuerto == null)
            return NotFound(new { mensaje = $"No existe un aeropuerto con id {id}." });

        return Ok(aeropuerto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AeropuertoDto>> Crear([FromBody] CrearAeropuertoDto dto)
    {
        var aeropuertoCreado = await _aeropuertoServicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = aeropuertoCreado.AeropuertoId }, aeropuertoCreado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarAeropuertoDto dto)
    {
        if (id != dto.AeropuertoId)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del cuerpo de la solicitud." });

        await _aeropuertoServicio.ActualizarAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _aeropuertoServicio.EliminarAsync(id);
        return NoContent();
    }
}
