using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistorialEstadosController : ControllerBase
{
    private readonly IHistorialEstadoVueloServicio _historialServicio;

    public HistorialEstadosController(IHistorialEstadoVueloServicio historialServicio)
    {
        _historialServicio = historialServicio;
    }

    [HttpGet("vuelo/{vueloId:int}")]
    public async Task<ActionResult<List<HistorialEstadoVueloDto>>> ObtenerPorVuelo(int vueloId)
    {
        var historial = await _historialServicio.ObtenerPorVueloAsync(vueloId);
        return Ok(historial);
    }
}
