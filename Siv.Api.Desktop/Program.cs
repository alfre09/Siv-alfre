using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Siv.Api.Desktop.Middleware;
using Siv.Application.Configuracion;
using Siv.Persistence;
using Siv.Persistence.Configuracion;
using Siv.Persistence.Semilla;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddControllers();

constructor.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = constructor.Configuration["Jwt:Issuer"]!,
            ValidAudience = constructor.Configuration["Jwt:Audience"]!,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                constructor.Configuration["Jwt:Key"]!))
        };
    });

constructor.Services.AddAuthorization(opciones =>
{
    opciones.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

constructor.Services.AddEndpointsApiExplorer();
constructor.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SIV - API Desktop",
        Version = "v1",
        Description = "API del Sistema de Información de Vuelos para el cliente de escritorio (WPF)."
    });
});

constructor.Services.AddSIVDependencies(constructor.Configuration);
constructor.Services.AddHttpContextAccessor();
constructor.Services.AddHttpClient<Siv.Application.Interfaces.INotificadorTiempoReal, Siv.Api.Desktop.Servicios.NotificadorApiWeb>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5200");
});
var aplicacion = constructor.Build();

if (aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseSwagger();
    aplicacion.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "SIV API Desktop v1");
    });
}

aplicacion.UseMiddleware<MiddlewareManejoDeExcepciones>();

aplicacion.UseHttpsRedirection();
aplicacion.UseAuthentication();
aplicacion.UseAuthorization();
aplicacion.MapControllers();

if (!aplicacion.Environment.IsEnvironment("Testing"))
{
    using var alcance = aplicacion.Services.CreateScope();
    var contexto = alcance.ServiceProvider.GetRequiredService<SivDbContext>();
    await contexto.Database.MigrateAsync();
    await SembradorDeDatos.InicializarAsync(contexto);
}

aplicacion.Run();

public partial class Program;
public class DesktopProgramMarker { }
