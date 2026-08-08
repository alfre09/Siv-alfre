using Siv.Domain.Enums;

namespace Siv.Domain.Entidades;

public static class PoliticaVisibilidadVuelo
{
    public static bool PuedeConsultar(NivelVisibilidad nivel, string? rolUsuario)
    {
        if (string.Equals(rolUsuario, "Admin", StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.Equals(rolUsuario, "Operador", StringComparison.OrdinalIgnoreCase))
            return nivel != NivelVisibilidad.Restringido;

        if (string.Equals(rolUsuario, "Auditor", StringComparison.OrdinalIgnoreCase))
            return nivel != NivelVisibilidad.Restringido;

        return nivel == NivelVisibilidad.Publico;
    }
}
