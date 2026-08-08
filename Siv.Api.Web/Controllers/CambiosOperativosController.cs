using Microsoft.AspNetCore.Mvc;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CambiosOperativosController : ControllerBase
{
    private readonly ICambioOperativoServicio _cambioOperativoServicio;

    public CambiosOperativosController(ICambioOperativoServicio cambioOperativoServicio)
    {
        _cambioOperativoServicio = cambioOperativoServicio;
    }

    [HttpGet("vuelo/{vueloId:int}")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<ActionResult<List<CambioOperativoDto>>> ObtenerPorVuelo(int vueloId)
    {
        var rolUsuario = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var cambios = await _cambioOperativoServicio.ObtenerPorVueloAsync(vueloId, rolUsuario);
        return Ok(cambios);
    }

}


