using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AerolineasController : ControllerBase
{
    private readonly IAerolineaServicio _aerolineaServicio;

    public AerolineasController(IAerolineaServicio aerolineaServicio)
    {
        _aerolineaServicio = aerolineaServicio;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador,Auditor")]
    [ProducesResponseType(typeof(List<AerolineaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AerolineaDto>>> ObtenerTodos()
    {
        var aerolineas = await _aerolineaServicio.ObtenerTodosAsync();
        return Ok(aerolineas);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Operador,Auditor")]
    [ProducesResponseType(typeof(AerolineaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AerolineaDto>> ObtenerPorId(int id)
    {
        var aerolinea = await _aerolineaServicio.ObtenerPorIdAsync(id);

        if (aerolinea == null)
            return NotFound(new { mensaje = $"No existe una aerolínea con id {id}." });

        return Ok(aerolinea);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AerolineaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AerolineaDto>> Crear([FromBody] CrearAerolineaDto dto)
    {
        var aerolineaCreada = await _aerolineaServicio.CrearAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = aerolineaCreada.AerolineaId }, aerolineaCreada);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarAerolineaDto dto)
    {
        if (id != dto.AerolineaId)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del cuerpo de la solicitud." });

        await _aerolineaServicio.ActualizarAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _aerolineaServicio.EliminarAsync(id);
        return NoContent();
    }
}
