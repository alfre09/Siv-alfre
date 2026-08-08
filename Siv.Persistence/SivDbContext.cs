using Microsoft.EntityFrameworkCore;
using Siv.Domain.Entidades;

namespace Siv.Persistence;

public class SivDbContext : DbContext
{
    public SivDbContext(DbContextOptions<SivDbContext> opciones) : base(opciones)
    {
    }

    public DbSet<Vuelo> Vuelos => Set<Vuelo>();
    public DbSet<Aerolinea> Aerolineas => Set<Aerolinea>();
    public DbSet<Aeropuerto> Aeropuertos => Set<Aeropuerto>();
    public DbSet<EstadoVuelo> EstadosVuelo => Set<EstadoVuelo>();
    public DbSet<Seguimiento> Seguimientos => Set<Seguimiento>();
    public DbSet<CambioOperativo> CambiosOperativos => Set<CambioOperativo>();
    public DbSet<HistorialEstadoVuelo> HistorialEstados => Set<HistorialEstadoVuelo>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SivDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
