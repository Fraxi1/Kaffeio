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

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configurazione HttpClient per l'API Kaffeio (basata sulla documentazione)
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

// Servizi di autenticazione
builder.Services.AddSingleton<ITokenService, BlazorTokenService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(
    provider => provider.GetService<CustomAuthenticationStateProvider>()!);

// Determina se usare servizi fake o reali
var useFakes = builder.Configuration.GetValue<bool>("UseFakes", true);

// Registrazione servizi API
if (useFakes)
{
    builder.Services.AddScoped<IAuthenticationService, AutoLoginService>();

    // Servizi fake per testing
    builder.Services.AddScoped<IUserService, FakeUserApiClient>();
    builder.Services.AddScoped<ILotService, FakeLotApiClient>();
    builder.Services.AddScoped<IMachineService, FakeMachineApiClient>();
    builder.Services.AddScoped<IFacilityService, FakeFacilityApiClient>();
    
    // AGGIUNTI: Nuovi servizi essenziali per CoffeeMek
    builder.Services.AddScoped<ICustomerService, FakeCustomerApiClient>();
    builder.Services.AddScoped<IOrderService, FakeOrderApiClient>();
    builder.Services.AddScoped<IProductionScheduleService, FakeProductionScheduleApiClient>();
   builder.Services.AddScoped<ITelemetryService, FakeTelemetryApiClient>();
}
else
{
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

    // Servizi reali per API Kaffeio
    builder.Services.AddScoped<IUserService, UserApiClient>();
    builder.Services.AddScoped<ILotService, LotApiClient>();
    builder.Services.AddScoped<IMachineService, MachineApiClient>();
    builder.Services.AddScoped<IFacilityService, FacilityApiClient>();
    
    // AGGIUNTI: Nuovi servizi reali per produzione
     builder.Services.AddScoped<ICustomerService, CustomerApiClient>();
    // builder.Services.AddScoped<IOrderService, OrderApiClient>();
    // builder.Services.AddScoped<IProductionScheduleService, ProductionScheduleApiClient>();
    // builder.Services.AddScoped<ITelemetryService, TelemetryApiClient>();
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
