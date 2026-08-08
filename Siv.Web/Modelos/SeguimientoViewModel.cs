using System.ComponentModel.DataAnnotations;

namespace Siv.Web.Modelos;

public class SeguimientoViewModel
{
    public int SeguimientoId { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public int VueloId { get; set; }
    public DateTime FechaSeguimiento { get; set; }
}

public class CrearSeguimientoViewModel
{
    [Required(ErrorMessage = "Indica tu nombre de usuario para seguir el vuelo.")]
    [Display(Name = "Usuario")]
    public string Usuario { get; set; } = string.Empty;

    public int VueloId { get; set; }
}
