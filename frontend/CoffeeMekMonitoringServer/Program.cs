using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using CoffeeMekMonitoringServer.Extensions;
using CoffeeMekMonitoringServer.Data;
using CoffeeMekMonitoringServer.Services;
using CoffeeMekMonitoringServer.Services.Interfaces;
using CoffeeMekMonitoringServer.Services.ApiClients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// HttpContextAccessor per eventuali usi futuri
builder.Services.AddHttpContextAccessor();

// Configurazione HttpClient per l'API Kaffeio
var backendUrl = builder.Configuration["Backend:Url"] ?? "http://localhost:3000";
builder.Services.AddHttpClient("CoffeeMekApi", client =>
{
    client.BaseAddress = new Uri(backendUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "CoffeeMek-Blazor/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Autorizzazione
builder.Services.AddAuthorizationCore();

// IMPORTANTE: Usa BlazorTokenService
builder.Services.AddSingleton<ITokenService, BlazorTokenService>(); // Singleton per mantenere lo stato
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(
    provider => provider.GetService<CustomAuthenticationStateProvider>()!);
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// UserService
var useFakes = builder.Configuration.GetValue<bool>("UseFakes", true);
if (useFakes)
{
    builder.Services.AddScoped<IUserService, FakeUserApiClient>();
}
else
{
    builder.Services.AddScoped<IUserService, UserApiClient>();
}

// Aggiungi servizi CoffeeMek esistenti
builder.Services.AddCoffeeMekServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
