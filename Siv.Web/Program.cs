using Microsoft.AspNetCore.Authentication.Cookies;
using Siv.Web.Configuracion;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddControllersWithViews();
constructor.Services.AgregarServiciosDeConsumoDeApi(constructor.Configuration);


constructor.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccesoDenegado";
    });

var aplicacion = constructor.Build();

if (!aplicacion.Environment.IsDevelopment())
    aplicacion.UseHsts();

// Captura excepciones no controladas también durante el desarrollo.
aplicacion.UseExceptionHandler("/Home/Error");

aplicacion.UseHttpsRedirection();
aplicacion.UseStaticFiles();

aplicacion.UseRouting();

aplicacion.UseAuthentication();
aplicacion.UseAuthorization();

aplicacion.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

aplicacion.Run();

