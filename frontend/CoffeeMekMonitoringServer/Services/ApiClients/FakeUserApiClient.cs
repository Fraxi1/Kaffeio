using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class FakeUserApiClient : IUserService
{
    private readonly List<User> _users;
    private int _nextId = 4;

    public FakeUserApiClient()
    {
        _users = new List<User>
        {
            new() { Id = 1, FirstName = "Mario", LastName = "Rossi", Email = "mario.rossi@coffeemek.com", Password = "password123" },
            new() { Id = 2, FirstName = "Anna", LastName = "Verdi", Email = "anna.verdi@coffeemek.com", Password = "password123" },
            new() { Id = 3, FirstName = "Luca", LastName = "Bianchi", Email = "luca.bianchi@coffeemek.com", Password = "password123" }
        };
    }

    public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
    {
        await Task.Delay(500); // Simula latenza di rete
        return ApiResponse<List<User>>.SuccessResult(new List<User>(_users));
    }

    public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
    {
        await Task.Delay(300);
        
        var user = _users.FirstOrDefault(u => u.Id == id);
        return user != null 
            ? ApiResponse<User>.SuccessResult(user)
            : ApiResponse<User>.ErrorResult("Utente non trovato", 404);
    }

    public async Task<ApiResponse<User>> CreateUserAsync(User user)
    {
        await Task.Delay(700);

        if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return ApiResponse<User>.ErrorResult("Email già esistente", 400);
        }

        var newUser = new User
        {
            Id = _nextId++,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = user.Password
        };

        _users.Add(newUser);
        return ApiResponse<User>.SuccessResult(newUser);
    }

    public async Task<ApiResponse<User>> UpdateUserAsync(int id, User user)
    {
        await Task.Delay(600);

        var existingUser = _users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return ApiResponse<User>.ErrorResult("Utente non trovato", 404);
        }

        if (_users.Any(u => u.Id != id && u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return ApiResponse<User>.ErrorResult("Email già esistente", 400);
        }

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        if (!string.IsNullOrEmpty(user.Password))
        {
            existingUser.Password = user.Password;
        }

        return ApiResponse<User>.SuccessResult(existingUser);
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
    {
        await Task.Delay(400);

        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return ApiResponse<bool>.ErrorResult("Utente non trovato", 404);
        }

        _users.Remove(user);
        return ApiResponse<bool>.SuccessResult(true);
    }
}
