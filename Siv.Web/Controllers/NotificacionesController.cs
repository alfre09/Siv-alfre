using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Siv.Web.Interfaces;

namespace Siv.Web.Controllers;

public class NotificacionesController : Controller
{
    private readonly INotificacionApiServicio _notificacionApiServicio;

    public NotificacionesController(INotificacionApiServicio notificacionApiServicio)
    {
        _notificacionApiServicio = notificacionApiServicio;
    }

    [Authorize(Roles = "UsuarioRegistrado")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuario = User.Identity?.Name;
        ViewData["Usuario"] = usuario;

        if (string.IsNullOrWhiteSpace(usuario))
        {
            return View(); // Se mostrará solo el formulario de búsqueda
        }

        var notificaciones = await _notificacionApiServicio.ObtenerPorUsuarioAsync(usuario);
        // Mostrar las más recientes primero
        var modelo = notificaciones.OrderByDescending(n => n.FechaEnvio).ToList();

        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "UsuarioRegistrado")]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        try
        {
            await _notificacionApiServicio.MarcarComoLeidaAsync(id);
        }
        catch (Exception)
        {
            TempData["Error"] = "No se pudo marcar la notificación como leída.";
        }

        return RedirectToAction(nameof(Index));
    }
}
