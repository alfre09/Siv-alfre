using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Siv.Domain.Entidades;
using Siv.Domain.Enums;
using Siv.Persistence;

namespace Siv.Tests.Integracion;

public sealed class SivApiWebFactory : WebApplicationFactory<WebProgramMarker>
{
    private readonly string _nombreBase;

    public SivApiWebFactory(string? nombreBase = null)
    {
        _nombreBase = nombreBase ?? $"SivTests_{Guid.NewGuid():N}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(servicios =>
        {
            var registrosDb = servicios
                .Where(registro =>
                    registro.ServiceType == typeof(SivDbContext) ||
                    registro.ServiceType == typeof(DbContextOptions) ||
                    registro.ServiceType == typeof(DbContextOptions<SivDbContext>) ||
                    registro.ServiceType.FullName?.Contains("IDbContextOptionsConfiguration") == true)
                .ToList();

            foreach (var registro in registrosDb)
                servicios.Remove(registro);

            servicios.AddDbContext<SivDbContext>(opciones =>
                opciones.UseInMemoryDatabase(_nombreBase));
        });
    }

    public async Task<int> CrearVueloAsync(bool conCambio = false)
    {
        using var alcance = Services.CreateScope();
        var contexto = alcance.ServiceProvider.GetRequiredService<SivDbContext>();
        await contexto.Database.EnsureCreatedAsync();

        var estado = new EstadoVuelo(EstadoVuelo.Programado);
        var aerolinea = new Aerolinea("IT", "Integration Test Airways");
        var origen = new Aeropuerto("TST", "Aeropuerto de prueba", "Pruebas", "Bolivia");
        var destino = new Aeropuerto("TST2", "Aeropuerto destino", "Pruebas", "Bolivia");

        contexto.EstadosVuelo.Add(estado);
        contexto.Aerolineas.Add(aerolinea);
        contexto.Aeropuertos.AddRange(origen, destino);
        await contexto.SaveChangesAsync();
        contexto.Puertas.AddRange(
            new Siv.Domain.Entidades.Puerta("A1", origen.Id),
            new Siv.Domain.Entidades.Puerta("B2", origen.Id),
            new Siv.Domain.Entidades.Puerta("C3", origen.Id),
            new Siv.Domain.Entidades.Puerta("D4", origen.Id));
        await contexto.SaveChangesAsync();

        var vuelo = new Vuelo(
            $"IT{Random.Shared.Next(10000, 99999)}",
            aerolinea.Id,
            origen.Id,
            destino.Id,
            DateTime.UtcNow.AddHours(2),
            estado.Id,
            "A1",
            NivelVisibilidad.Publico);
        contexto.Vuelos.Add(vuelo);
        await contexto.SaveChangesAsync();

        if (conCambio)
        {
            contexto.CambiosOperativos.Add(new CambioOperativo(
                vuelo.Id,
                TipoCambioOperativo.CambioPuerta,
                "Cambio de prueba",
                "A1",
                "B2"));
            await contexto.SaveChangesAsync();
        }

        return vuelo.Id;
    }
}
