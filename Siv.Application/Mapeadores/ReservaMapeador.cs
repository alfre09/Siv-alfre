using Siv.Application.Dtos;
using Siv.Domain.Entidades;

namespace Siv.Application.Mapeadores;

public static class ReservaMapeador
{
    public static ReservaDto ADto(this Reserva entidad)
    {
        return new ReservaDto
        {
            ReservaId = entidad.Id,
            Usuario = entidad.Usuario,
            VueloId = entidad.VueloId,
            NumeroVuelo = entidad.Vuelo?.NumeroVuelo ?? string.Empty,
            Origen = entidad.Vuelo?.AeropuertoOrigen?.Nombre,
            Destino = entidad.Vuelo?.AeropuertoDestino?.Nombre,
            HorarioProgramado = entidad.Vuelo?.HorarioProgramado ?? default,
            FechaReserva = entidad.FechaReserva,
            Estado = entidad.Estado.ToString()
        };
    }
}
