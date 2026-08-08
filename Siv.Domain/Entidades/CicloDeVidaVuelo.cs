using Siv.Domain.Excepciones;

namespace Siv.Domain.Entidades;

public static class CicloDeVidaVuelo
{
    public static void ValidarTransicion(EstadoVuelo estadoActual, EstadoVuelo estadoNuevo)
    {
        if (estadoActual.EsFinal())
            throw new ExcepcionDeDominio(
                $"El vuelo se encuentra en un estado final ('{estadoActual.Nombre}') y no puede continuar su ciclo operativo.");

        if (estadoNuevo.EsCancelado())
            return;

        var secuencia = EstadoVuelo.SecuenciaOperativa;

        var indiceActual = Array.FindIndex(
            secuencia, s => s.Equals(estadoActual.Nombre, StringComparison.OrdinalIgnoreCase));
        var indiceNuevo = Array.FindIndex(
            secuencia, s => s.Equals(estadoNuevo.Nombre, StringComparison.OrdinalIgnoreCase));

        if (indiceActual == -1 || indiceNuevo == -1)
            throw new ExcepcionDeDominio(
                $"Estado no reconocido en la secuencia operativa del vuelo ('{estadoActual.Nombre}' -> '{estadoNuevo.Nombre}').");

        if (indiceNuevo != indiceActual + 1)
            throw new ExcepcionDeDominio(
                $"No se puede pasar de '{estadoActual.Nombre}' a '{estadoNuevo.Nombre}' sin cumplir el estado anterior.");
    }
}
