namespace Siv.Desktop.Servicios;

public static class TokenManager
{
    public static string? Token { get; set; }
    public static string Rol { get; set; } = string.Empty;

    public static void Limpiar()
    {
        Token = null;
        Rol = string.Empty;
    }
}
