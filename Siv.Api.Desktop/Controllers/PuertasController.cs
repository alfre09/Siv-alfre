using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siv.Domain.Repositorios;

namespace Siv.Api.Desktop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Operador,Administrador")]
public class PuertasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public PuertasController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var puertas = await _unitOfWork.Puertas.ObtenerTodosAsync();
        return Ok(puertas.Select(p => new
        {
            p.Id,
            Codigo = p.Nombre,
            Estado = "Operativa"
        }));
    }
}
