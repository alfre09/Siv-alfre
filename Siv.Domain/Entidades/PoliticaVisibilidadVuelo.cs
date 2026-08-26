using Siv.Domain.Enums;

namespace Siv.Domain.Entidades;

public static class PoliticaVisibilidadVuelo
{
    public static bool PuedeConsultar(
        NivelVisibilidad nivel,
        string? rolUsuario,
        EstadoVuelo? estadoVuelo = null)
    {
        if (string.Equals(rolUsuario, "Admin", StringComparison.OrdinalIgnoreCase))
            return true;

        // Los vuelos en estado final dejan de formar parte de la consulta pública.
        // Los perfiles internos conservan acceso según su nivel de visibilidad para
        // supervisión y trazabilidad.
        var consultaPublica = string.IsNullOrWhiteSpace(rolUsuario)
            || string.Equals(rolUsuario, "UsuarioRegistrado", StringComparison.OrdinalIgnoreCase);

        if (consultaPublica && estadoVuelo?.EsFinal() == true)
            return false;

        if (string.Equals(rolUsuario, "Operador", StringComparison.OrdinalIgnoreCase))
            return nivel != NivelVisibilidad.Restringido;

        if (string.Equals(rolUsuario, "Auditor", StringComparison.OrdinalIgnoreCase))
            return nivel != NivelVisibilidad.Restringido;

        return nivel == NivelVisibilidad.Publico;
    }
}
