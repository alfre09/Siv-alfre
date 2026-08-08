using Microsoft.AspNetCore.Mvc;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AerolineasController : ControllerBase
{
    private readonly IAerolineaServicio _aerolineaServicio;

    public AerolineasController(IAerolineaServicio aerolineaServicio)
    {
        _aerolineaServicio = aerolineaServicio;
    }

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<List<AerolineaDto>>> ObtenerTodos()
    {
        var aerolineas = await _aerolineaServicio.ObtenerTodosAsync();
        return Ok(aerolineas);
    }

    [HttpGet("{id:int}")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<AerolineaDto>> ObtenerPorId(int id)
    {
        var aerolinea = await _aerolineaServicio.ObtenerPorIdAsync(id);

        if (aerolinea == null)
            return NotFound(new { mensaje = $"No existe una aerolínea con id {id}." });

        return Ok(aerolinea);
    }

}


