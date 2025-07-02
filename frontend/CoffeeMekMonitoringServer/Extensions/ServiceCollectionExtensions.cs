using CoffeeMekMonitoringServer.Services.ApiClients;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoffeeMekServices(this IServiceCollection services, IConfiguration configuration)
    {
        var useFakes = configuration.GetValue<bool>("UseFakes");
        var backendUrl = configuration.GetValue<string>("Backend:Url");

        // Configurazione HttpClient per API backend
        services.AddHttpClient("CoffeeMekApi", client =>
        {
            client.BaseAddress = new Uri(backendUrl ?? "http://localhost:3000/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // Registrazione servizi condizionale
        if (useFakes)
        {
            services.AddScoped<IUserService, FakeUserApiClient>();
        }
        else
        {
            services.AddScoped<IUserService, UserApiClient>();
        }

        return services;
    }
}