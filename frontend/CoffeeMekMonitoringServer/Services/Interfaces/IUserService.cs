using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<List<User>>> GetAllUsersAsync();
    Task<ApiResponse<User>> GetUserByIdAsync(int id);
    Task<ApiResponse<User>> CreateUserAsync(User user);
    Task<ApiResponse<User>> UpdateUserAsync(int id, User user);
    Task<ApiResponse<bool>> DeleteUserAsync(int id);
}