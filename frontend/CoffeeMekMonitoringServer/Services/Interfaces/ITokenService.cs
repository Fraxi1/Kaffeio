using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface ITokenService
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task RemoveTokenAsync();
    Task<bool> HasTokenAsync();
    Task SetUserAsync(User user);
    Task<User?> GetUserAsync();
    Task RemoveUserAsync();
}