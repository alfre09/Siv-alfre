using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstadosVueloController : ControllerBase
{
    private readonly IEstadoVueloServicio _estadoVueloServicio;

    public EstadosVueloController(IEstadoVueloServicio estadoVueloServicio)
    {
        _estadoVueloServicio = estadoVueloServicio;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Operador,Auditor")]
    public async Task<ActionResult<List<EstadoVueloDto>>> ObtenerTodos()
    {
        var estados = await _estadoVueloServicio.ObtenerTodosAsync();
        return Ok(estados);
    }
}
