using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Controllers;

public class VuelosController : Controller
{
    private readonly IVueloApiServicio _vueloApiServicio;
    private readonly ICambioOperativoApiServicio _cambioOperativoApiServicio;
    private readonly ISeguimientoApiServicio _seguimientoApiServicio;
    private readonly ILogger<VuelosController> _logger;

    public VuelosController(
        IVueloApiServicio vueloApiServicio,
        ICambioOperativoApiServicio cambioOperativoApiServicio,
        ISeguimientoApiServicio seguimientoApiServicio,
        ILogger<VuelosController> logger)
    {
        _vueloApiServicio = vueloApiServicio;
        _cambioOperativoApiServicio = cambioOperativoApiServicio;
        _seguimientoApiServicio = seguimientoApiServicio;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        try
        {
            var vuelos = await _vueloApiServicio.ObtenerTodosAsync();
            return View(vuelos);
        }
        catch (ExcepcionApi excepcion)
        {
            _logger.LogWarning(excepcion, "No se pudo obtener el listado de vuelos");
            TempData["MensajeError"] = excepcion.Message;
            return View(new List<VueloViewModel>());
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Detalle(int id)
    {
        try
        {
            var vuelo = await _vueloApiServicio.ObtenerPorIdAsync(id);

            if (vuelo == null)
                return NotFound();

            var cambios = await _cambioOperativoApiServicio.ObtenerPorVueloAsync(id);

            var modelo = new DetalleVueloViewModel
            {
                Vuelo = vuelo,
                Cambios = cambios
            };

            return View(modelo);
        }
        catch (ExcepcionApi excepcion)
        {
            _logger.LogWarning(excepcion, "No se pudo obtener el detalle del vuelo {VueloId}", id);
            TempData["MensajeError"] = excepcion.Message;
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "UsuarioRegistrado,Admin")]
    public async Task<IActionResult> Seguir(int vueloId)
    {
        var usuarioNormalizado = User.Identity?.Name?.Trim();

        if (string.IsNullOrWhiteSpace(usuarioNormalizado))
        {
            TempData["MensajeError"] = "No se pudo identificar al usuario autenticado.";
            return RedirectToAction(nameof(Detalle), new { id = vueloId });
        }

        try
        {
            await _seguimientoApiServicio.CrearAsync(new CrearSeguimientoViewModel
            {
                Usuario = usuarioNormalizado,
                VueloId = vueloId
            });
            TempData["MensajeExito"] = "Ahora estás siguiendo este vuelo. Te notificaremos ante cualquier cambio.";
        }
        catch (ExcepcionApi excepcion)
        {
            TempData["MensajeError"] = excepcion.Message;
        }

        return RedirectToAction(nameof(Detalle), new { id = vueloId });
    }
}
