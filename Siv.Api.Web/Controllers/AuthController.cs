using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Siv.Application.Dtos;
using Siv.Application.Interfaces;

namespace Siv.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUsuarioServicio _usuarioServicio;

    public AuthController(IConfiguration configuration, IUsuarioServicio usuarioServicio)
    {
        _configuration = configuration;
        _usuarioServicio = usuarioServicio;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuarioStr = request.Usuario.Trim().ToLowerInvariant();
        var usuarioRegistrado = await _usuarioServicio.ValidarCredencialesAsync(usuarioStr, request.Password);

        var rol = usuarioRegistrado?.Rol;
        var nombreToken = usuarioRegistrado?.NombreUsuario ?? usuarioStr;

        if (string.IsNullOrWhiteSpace(rol))
        {
            var configuracionUsuario = _configuration.GetSection($"Authentication:Users:{usuarioStr}");
            var passwordEsperada = configuracionUsuario["Password"];
            rol = configuracionUsuario["Role"];

            if (string.IsNullOrWhiteSpace(passwordEsperada) ||
                string.IsNullOrWhiteSpace(rol) ||
                !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(request.Password ?? string.Empty),
                    Encoding.UTF8.GetBytes(passwordEsperada)))
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            }
        }

        if (!string.Equals(rol, "UsuarioRegistrado", StringComparison.OrdinalIgnoreCase))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { mensaje = "La API Web está destinada únicamente a clientes registrados." });

        return CrearRespuestaToken(nombreToken, rol);
    }

    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        var nombre = dto.NombreUsuario?.Trim().ToLowerInvariant() ?? string.Empty;
        var usuarioConfigurado = _configuration.GetSection($"Authentication:Users:{nombre}");
        if (!string.IsNullOrWhiteSpace(usuarioConfigurado["Role"]))
            return BadRequest(new { mensaje = "Ese nombre está reservado por el sistema." });

        await _usuarioServicio.RegistrarAsync(dto);
        return StatusCode(StatusCodes.Status201Created, new { mensaje = "Usuario registrado correctamente." });
    }

    private IActionResult CrearRespuestaToken(string nombreUsuario, string rol)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, nombreUsuario),
            new Claim(ClaimTypes.Role, rol)
        };

        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "Siv.Api",
            audience: jwtSettings["Audience"] ?? "Siv.Clients",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return Ok(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Rol = rol
        });
    }
}

public class LoginRequest
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
