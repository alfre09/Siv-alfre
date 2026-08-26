using Siv.Domain.Entidades;
using Siv.Domain.Enums;

namespace Siv.Tests.Domain;

public class PoliticaVisibilidadVueloTests
{
    [Fact]
    public void Visitante_NoDebeConsultarVuelosEnEstadoFinal()
    {
        var estado = new EstadoVuelo(EstadoVuelo.Cancelado);

        var resultado = PoliticaVisibilidadVuelo.PuedeConsultar(
            NivelVisibilidad.Publico,
            rolUsuario: null,
            estado);

        Assert.False(resultado);
    }

    [Fact]
    public void UsuarioRegistrado_DebeConsultarVueloPublicoActivo()
    {
        var estado = new EstadoVuelo(EstadoVuelo.Embarcando);

        var resultado = PoliticaVisibilidadVuelo.PuedeConsultar(
            NivelVisibilidad.Publico,
            "UsuarioRegistrado",
            estado);

        Assert.True(resultado);
    }

    [Fact]
    public void Operador_DebeConsultarVueloInternoAunqueEsteEnEstadoFinal()
    {
        var estado = new EstadoVuelo(EstadoVuelo.Aterrizado);

        var resultado = PoliticaVisibilidadVuelo.PuedeConsultar(
            NivelVisibilidad.Interno,
            "Operador",
            estado);

        Assert.True(resultado);
    }
}
