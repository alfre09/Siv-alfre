using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Controllers;

public class DisponiblesController : Controller
{
    private readonly IVueloApiServicio _vueloApiServicio;
    private readonly IAeropuertoApiServicio _aeropuertoApiServicio;
    private readonly ILogger<DisponiblesController> _logger;

    public DisponiblesController(
        IVueloApiServicio vueloApiServicio,
        IAeropuertoApiServicio aeropuertoApiServicio,
        ILogger<DisponiblesController> logger)
    {
        _vueloApiServicio = vueloApiServicio;
        _aeropuertoApiServicio = aeropuertoApiServicio;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int? origenId, int? destinoId, DateTime? fecha)
    {
        try
        {
            var vuelos = await _vueloApiServicio.ObtenerDisponiblesAsync(origenId, destinoId, fecha);
            var aeropuertos = await _aeropuertoApiServicio.ObtenerTodosAsync();
            return View(new DisponiblesViewModel
            {
                Vuelos = vuelos,
                Aeropuertos = aeropuertos,
                OrigenId = origenId,
                DestinoId = destinoId,
                Fecha = fecha
            });
        }
        catch (ExcepcionApi excepcion)
        {
            _logger.LogWarning(excepcion, "No se pudieron obtener los vuelos disponibles");
            TempData["MensajeError"] = excepcion.Message;
            return View(new DisponiblesViewModel { OrigenId = origenId, DestinoId = destinoId, Fecha = fecha });
        }
    }
}
