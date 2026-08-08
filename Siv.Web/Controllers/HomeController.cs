using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;
using Siv.Web.Models;

namespace Siv.Web.Controllers;

public class HomeController : Controller
{
    private readonly IVueloApiServicio _vueloApiServicio;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IVueloApiServicio vueloApiServicio, ILogger<HomeController> logger)
    {
        _vueloApiServicio = vueloApiServicio;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var vuelos = await _vueloApiServicio.ObtenerTodosAsync();
            var proximosVuelos = vuelos
                .OrderBy(v => v.HorarioProgramado)
                .Take(6)
                .ToList();

            return View(proximosVuelos);
        }
        catch (ExcepcionApi excepcion)
        {
            _logger.LogWarning(excepcion, "No se pudo obtener el tablero de vuelos para la portada");
            return View(new List<VueloViewModel>());
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
