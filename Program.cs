using tl2_tp8_2025_ElAguhs.Interfaces;
using tl2_tp8_2025_ElAguhs.Repositorios;
using tl2_tp8_2025_ElAguhs.Services; 
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor(); 
builder.Services.AddSession(options => 
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; 
});


builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IPresupuestoRepository, PresupuestosRepository>();
builder.Services.AddScoped<IUserRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();


app.UseSession(); 
app.UseHttpsRedirection(); 
app.UseStaticFiles(); 
app.UseRouting(); 
app.UseAuthorization(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();