using Siv.Application.Dtos;
using Siv.Domain.Entidades;
using Siv.Domain.Enums;

namespace Siv.Application.Mapeadores;

public static class CambioOperativoMapeador
{
    public static CambioOperativoDto ADto(this CambioOperativo entidad)
    {
        return new CambioOperativoDto
        {
            CambioOperativoId = entidad.Id,
            VueloId = entidad.VueloId,
            TipoCambio = entidad.TipoCambio.ANombreLegible(),
            Causa = entidad.Causa,
            ValorAnterior = entidad.ValorAnterior,
            ValorNuevo = entidad.ValorNuevo,
            FechaCambio = entidad.FechaCambio
        };
    }

    public static string ANombreLegible(this TipoCambioOperativo tipo)
    {
        return tipo switch
        {
            TipoCambioOperativo.Retraso => "Retraso",
            TipoCambioOperativo.Adelanto => "Adelanto",
            TipoCambioOperativo.CambioPuerta => "Cambio de puerta",
            TipoCambioOperativo.CambioEstado => "Cambio de estado",
            TipoCambioOperativo.Cancelacion => "Cancelación",
            TipoCambioOperativo.CambioAerolinea => "Cambio de aerolínea",
            TipoCambioOperativo.CambioRuta => "Cambio de ruta",
            _ => tipo.ToString()
        };
    }
}
