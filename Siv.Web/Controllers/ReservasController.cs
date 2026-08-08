using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Controllers;

[Authorize(Roles = "UsuarioRegistrado,Admin")]
public class ReservasController : Controller
{
    private readonly IReservaApiServicio _reservaApiServicio;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(IReservaApiServicio reservaApiServicio, ILogger<ReservasController> logger)
    {
        _reservaApiServicio = reservaApiServicio;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await _reservaApiServicio.ObtenerMisReservasAsync());
        }
        catch (ExcepcionApi excepcion)
        {
            _logger.LogWarning(excepcion, "No se pudieron obtener las reservas del usuario");
            TempData["MensajeError"] = excepcion.Message;
            return View(new List<ReservaViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(int vueloId)
    {
        try
        {
            await _reservaApiServicio.CrearAsync(vueloId);
            TempData["MensajeExito"] = "La reserva se creó correctamente.";
        }
        catch (ExcepcionApi excepcion)
        {
            _logger.LogWarning(excepcion, "No se pudo crear la reserva del vuelo {VueloId}", vueloId);
            TempData["MensajeError"] = excepcion.Message;
        }

        return RedirectToAction("Detalle", "Vuelos", new { id = vueloId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        try
        {
            await _reservaApiServicio.CancelarAsync(id);
            TempData["MensajeExito"] = "La reserva se canceló correctamente.";
        }
        catch (ExcepcionApi excepcion)
        {
            _logger.LogWarning(excepcion, "No se pudo cancelar la reserva {ReservaId}", id);
            TempData["MensajeError"] = excepcion.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
