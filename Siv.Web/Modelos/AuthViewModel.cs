using System.ComponentModel.DataAnnotations;

namespace Siv.Web.Modelos;

public class LoginViewModel
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseViewModel
{
    public string Token { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}

public class RegistroViewModel
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres.")]
    [RegularExpression("^[a-zA-Z0-9._-]+$", ErrorMessage = "El usuario solo puede usar letras, números, punto, guion o guion bajo.")]
    [Display(Name = "Usuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Indica un correo electrónico válido.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma la contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;
}
