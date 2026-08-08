using Siv.Domain.Enums;

namespace Siv.Domain.Entidades;

public class CambioOperativo : EntidadBase
{
    protected CambioOperativo()
    {
        Causa = string.Empty;
        ValorAnterior = string.Empty;
        ValorNuevo = string.Empty;
    }

    public CambioOperativo(int vueloId, TipoCambioOperativo tipoCambio, string causa, string valorAnterior, string valorNuevo)
    {
        if (string.IsNullOrWhiteSpace(causa))
            throw new ArgumentException("Todo cambio operativo debe tener una causa identificable.", nameof(causa));

        VueloId = vueloId;
        TipoCambio = tipoCambio;
        Causa = causa.Trim();
        ValorAnterior = valorAnterior ?? string.Empty;
        ValorNuevo = valorNuevo ?? string.Empty;
        FechaCambio = DateTime.UtcNow;
    }

    public int VueloId { get; private set; }

    public Vuelo? Vuelo { get; private set; }

    public TipoCambioOperativo TipoCambio { get; private set; }

    public string Causa { get; private set; }

    public string ValorAnterior { get; private set; }

    public string ValorNuevo { get; private set; }

    public DateTime FechaCambio { get; private set; }
}
