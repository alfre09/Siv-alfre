using Microsoft.AspNetCore.Mvc;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = "UsuarioRegistrado")]
public class SeguimientosController : ControllerBase
{
    private readonly ISeguimientoServicio _seguimientoServicio;

    public SeguimientosController(ISeguimientoServicio seguimientoServicio)
    {
        _seguimientoServicio = seguimientoServicio;
    }

    [HttpPost]
    public async Task<ActionResult<SeguimientoDto>> Crear([FromBody] CrearSeguimientoDto dto)
    {
        var usuario = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(usuario))
            return Unauthorized();

        dto.Usuario = usuario;
        var seguimientoCreado = await _seguimientoServicio.CrearAsync(dto);
        return Ok(seguimientoCreado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _seguimientoServicio.EliminarAsync(id);
        return NoContent();
    }

    [HttpGet("{vueloId:int}/verificar")]
    public async Task<ActionResult<SeguimientoDto>> VerificarSeguimiento(int vueloId, [FromQuery] string usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario))
            return BadRequest(new { mensaje = "El usuario es obligatorio." });

        var seguimiento = await _seguimientoServicio.ObtenerPorVueloYUsuarioAsync(vueloId, usuario);
        
        if (seguimiento == null)
            return NotFound();

        return Ok(seguimiento);
    }
}


