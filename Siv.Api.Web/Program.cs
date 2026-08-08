using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Siv.Api.Web.Middleware;
using Siv.Api.Web.Hubs;
using Siv.Api.Web.Servicios;
using Siv.Application.Interfaces;
using Siv.Application.Configuracion;
using Siv.Persistence;
using Siv.Persistence.Configuracion;
using Siv.Persistence.Semilla;

var constructor = WebApplication.CreateBuilder(args);


constructor.Services.AddControllers(options => options.Filters.Add<Siv.Api.Web.Filtros.AuditoriaLecturaFilter>());

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

constructor.Services.AddEndpointsApiExplorer();
constructor.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SIV - API Web",
        Version = "v1",
        Description = "API del Sistema de Información de Vuelos para el cliente web (ASP.NET Core MVC)."
    });
});

constructor.Services.AddSignalR();
constructor.Services.AddScoped<INotificadorTiempoReal, NotificadorSignalR>();

constructor.Services.AddCors(options =>
{
    options.AddPolicy("AllowSivWeb", builder =>
    {
        builder.WithOrigins("http://localhost:5100", "https://localhost:7100")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

constructor.Services.AddSIVDependencies(constructor.Configuration);

var aplicacion = constructor.Build();

if (aplicacion.Environment.IsDevelopment())
{
    aplicacion.UseSwagger();
    aplicacion.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "SIV API Web v1");
    });
}

aplicacion.UseMiddleware<MiddlewareManejoDeExcepciones>();

aplicacion.UseHttpsRedirection();
aplicacion.UseCors("AllowSivWeb");
aplicacion.UseAuthentication();
aplicacion.UseAuthorization();
aplicacion.MapControllers();
aplicacion.MapHub<NotificacionesHub>("/notificacionesHub");

if (!aplicacion.Environment.IsEnvironment("Testing"))
{
    using var alcance = aplicacion.Services.CreateScope();
    var contexto = alcance.ServiceProvider.GetRequiredService<SivDbContext>();
    await contexto.Database.MigrateAsync();
    await SembradorDeDatos.InicializarAsync(contexto);
}

aplicacion.Run();

public partial class Program;
public class WebProgramMarker { }
