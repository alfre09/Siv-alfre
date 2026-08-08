using Siv.Desktop.Modelos;

namespace Siv.Desktop.Interfaces;

public interface IPuertaApiServicio
{
    Task<List<PuertaModelo>> ObtenerTodasAsync();
}
