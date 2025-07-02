using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<List<User>>> GetAllUsersAsync();
    Task<ApiResponse<User>> GetUserByIdAsync(int id); // CAMBIATO: int
    Task<ApiResponse<User>> CreateUserAsync(User user);
    Task<ApiResponse<User>> UpdateUserAsync(int id, User user); // CAMBIATO: int
    Task<ApiResponse<bool>> DeleteUserAsync(int id); // CAMBIATO: int
}