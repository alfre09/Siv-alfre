namespace Siv.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool MostrarIdSolicitud => !string.IsNullOrEmpty(RequestId);
}
