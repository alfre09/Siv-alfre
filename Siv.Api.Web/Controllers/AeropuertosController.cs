using Microsoft.AspNetCore.Mvc;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AeropuertosController : ControllerBase
{
    private readonly IAeropuertoServicio _aeropuertoServicio;

    public AeropuertosController(IAeropuertoServicio aeropuertoServicio)
    {
        _aeropuertoServicio = aeropuertoServicio;
    }

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<List<AeropuertoDto>>> ObtenerTodos()
    {
        var aeropuertos = await _aeropuertoServicio.ObtenerTodosAsync();
        return Ok(aeropuertos);
    }

    [HttpGet("{id:int}")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<AeropuertoDto>> ObtenerPorId(int id)
    {
        var aeropuerto = await _aeropuertoServicio.ObtenerPorIdAsync(id);

        if (aeropuerto == null)
            return NotFound(new { mensaje = $"No existe un aeropuerto con id {id}." });

        return Ok(aeropuerto);
    }

}


