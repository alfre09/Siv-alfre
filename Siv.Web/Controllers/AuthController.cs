using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Siv.Web.Interfaces;
using Siv.Web.Modelos;

namespace Siv.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthApiServicio _authApiServicio;

    public AuthController(IAuthApiServicio authApiServicio)
    {
        _authApiServicio = authApiServicio;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Registro() => View(new RegistroViewModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        try
        {
            var resultado = await _authApiServicio.LoginAsync(modelo);

            if (!string.Equals(resultado.Rol, "UsuarioRegistrado", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty,
                    "La Web está destinada a clientes registrados. Los roles operativos deben usar la aplicación Desktop.");
                return View(modelo);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, modelo.Usuario.Trim()),
                new(ClaimTypes.Role, resultado.Rol),
                new("jwt", resultado.Token)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            return RedirectToAction("Index", "Home");
        }
        catch (ExcepcionApi excepcion)
        {
            ModelState.AddModelError(string.Empty,
                excepcion.CodigoEstado == 401
                    ? "Usuario o contraseña incorrectos."
                    : excepcion.Message);
            return View(modelo);
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        try
        {
            await _authApiServicio.RegistrarAsync(modelo);
            TempData["MensajeExito"] = "Cuenta creada correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }
        catch (ExcepcionApi excepcion)
        {
            ModelState.AddModelError(string.Empty, excepcion.Message);
            return View(modelo);
        }
    }

    [HttpGet]
    public IActionResult AccesoDenegado() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
