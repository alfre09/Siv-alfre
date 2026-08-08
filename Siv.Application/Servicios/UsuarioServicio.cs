using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Siv.Application.Dtos;
using Siv.Application.Excepciones;
using Siv.Application.Interfaces;
using Siv.Application.Seguridad;
using Siv.Domain.Entidades;
using Siv.Domain.Repositorios;

namespace Siv.Application.Servicios;

public class UsuarioServicio : IUsuarioServicio
{
    private static readonly Regex NombreValido = new("^[a-zA-Z0-9._-]+$", RegexOptions.Compiled);
    private readonly IUnitOfWork _unidadDeTrabajo;
    private readonly IAuditoriaServicio _auditoriaServicio;
    private readonly ILogger<UsuarioServicio> _logger;

    public UsuarioServicio(
        IUnitOfWork unidadDeTrabajo,
        IAuditoriaServicio auditoriaServicio,
        ILogger<UsuarioServicio> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _auditoriaServicio = auditoriaServicio;
        _logger = logger;
    }

    public async Task RegistrarAsync(RegistrarUsuarioDto dto)
    {
        var nombre = dto.NombreUsuario?.Trim().ToLowerInvariant() ?? string.Empty;
        ValidarRegistro(nombre, dto.Password);

        if (await _unidadDeTrabajo.Usuarios.ObtenerPorNombreAsync(nombre) is not null)
            throw new ExcepcionDeValidacion("Ese nombre de usuario ya está registrado.");

        var usuario = new Usuario(nombre, "UsuarioRegistrado");
        usuario.EstablecerPasswordHash(PasswordHasher.Crear(dto.Password));

        await _unidadDeTrabajo.Usuarios.AgregarAsync(usuario);
        await _unidadDeTrabajo.GuardarCambiosAsync();

        await _auditoriaServicio.RegistrarAsync(
            "Crear", "Usuarios", $"Se registró el usuario {nombre} como UsuarioRegistrado.");
    }

    public async Task<UsuarioAutenticacionDto?> ValidarCredencialesAsync(string nombreUsuario, string password)
    {
        var nombre = nombreUsuario?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrEmpty(password))
            return null;

        var usuario = await _unidadDeTrabajo.Usuarios.ObtenerPorNombreAsync(nombre);
        if (usuario is null || !usuario.Activo || string.IsNullOrWhiteSpace(usuario.PasswordHash) ||
            !PasswordHasher.Verificar(password, usuario.PasswordHash))
            return null;

        return new UsuarioAutenticacionDto
        {
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol
        };
    }

    private void ValidarRegistro(string nombre, string password)
    {
        if (nombre.Length < 3 || nombre.Length > 50 || !NombreValido.IsMatch(nombre))
            throw new ExcepcionDeValidacion("El usuario debe tener entre 3 y 50 caracteres y solo puede usar letras, números, punto, guion o guion bajo.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8 ||
            !password.Any(char.IsUpper) || !password.Any(char.IsLower) || !password.Any(char.IsDigit))
            throw new ExcepcionDeValidacion("La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.");
    }
}
