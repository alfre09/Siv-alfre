using Microsoft.AspNetCore.Mvc;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadosVueloController : ControllerBase
{
    private readonly IEstadoVueloServicio _estadoVueloServicio;

    public EstadosVueloController(IEstadoVueloServicio estadoVueloServicio)
    {
        _estadoVueloServicio = estadoVueloServicio;
    }

    [HttpGet]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<List<EstadoVueloDto>>> ObtenerTodos()
    {
        var estados = await _estadoVueloServicio.ObtenerTodosAsync();
        return Ok(estados);
    }
}


