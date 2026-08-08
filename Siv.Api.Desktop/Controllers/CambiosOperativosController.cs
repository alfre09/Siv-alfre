using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CambiosOperativosController : ControllerBase
{
    private readonly ICambioOperativoServicio _cambioOperativoServicio;

    public CambiosOperativosController(ICambioOperativoServicio cambioOperativoServicio)
    {
        _cambioOperativoServicio = cambioOperativoServicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<CambioOperativoDto>>> ObtenerTodos()
    {
        var cambios = await _cambioOperativoServicio.ObtenerTodosAsync();
        return Ok(cambios);
    }

    [HttpGet("vuelo/{vueloId:int}")]
    public async Task<ActionResult<List<CambioOperativoDto>>> ObtenerPorVuelo(int vueloId)
    {
        var rolUsuario = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var cambios = await _cambioOperativoServicio.ObtenerPorVueloAsync(vueloId, rolUsuario);
        return Ok(cambios);
    }

    [HttpPost("horario")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<CambioOperativoDto>> RegistrarRetrasoOAdelanto(
        [FromBody] RegistrarRetrasoOAdelantoDto dto)
    {
        var cambio = await _cambioOperativoServicio.RegistrarRetrasoOAdelantoAsync(dto);
        return Ok(cambio);
    }

    [HttpPost("puerta")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<CambioOperativoDto>> RegistrarCambioPuerta(
        [FromBody] RegistrarCambioPuertaDto dto)
    {
        var cambio = await _cambioOperativoServicio.RegistrarCambioPuertaAsync(dto);
        return Ok(cambio);
    }

    [HttpPost("estado")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<CambioOperativoDto>> RegistrarCambioEstado(
        [FromBody] RegistrarCambioEstadoDto dto)
    {
        var cambio = await _cambioOperativoServicio.RegistrarCambioEstadoAsync(dto);
        return Ok(cambio);
    }

    [HttpPost("cancelacion")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<CambioOperativoDto>> RegistrarCancelacion(
        [FromBody] RegistrarCancelacionDto dto)
    {
        var cambio = await _cambioOperativoServicio.RegistrarCancelacionAsync(dto);
        return Ok(cambio);
    }
}
