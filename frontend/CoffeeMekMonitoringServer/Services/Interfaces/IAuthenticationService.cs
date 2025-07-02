using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface IAuthenticationService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<User?> GetCurrentUserAsync();
}