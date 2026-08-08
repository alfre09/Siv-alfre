using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditoriasController : ControllerBase
{
    private readonly IAuditoriaServicio _auditoriaServicio;

    public AuditoriasController(IAuditoriaServicio auditoriaServicio)
    {
        _auditoriaServicio = auditoriaServicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuditoriaDto>>> ObtenerTodos()
    {
        var auditorias = await _auditoriaServicio.ObtenerTodosAsync();
        return Ok(auditorias);
    }

    [HttpGet("tabla/{tabla}")]
    public async Task<ActionResult<List<AuditoriaDto>>> ObtenerPorTabla(string tabla)
    {
        var auditorias = await _auditoriaServicio.ObtenerPorTablaAsync(tabla);
        return Ok(auditorias);
    }
}
