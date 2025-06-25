using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class FakeUserApiClient : IUserService
{
    private readonly List<User> _users;
    private int _nextIdCounter = 4;

    public FakeUserApiClient()
    {
        _users = new List<User>
        {
            new() { 
                Id = 1, 
                FirstName = "Mario", 
                LastName = "Rossi", 
                Email = "mario.rossi@coffeemek.com", 
                Password = "password123",
                CreatedAt = DateTime.UtcNow.AddDays(-10).ToString(),
                UpdatedAt = DateTime.UtcNow.AddDays(-2).ToString(),
            },
            new() { 
                Id = 2, 
                FirstName = "Anna", 
                LastName = "Verdi", 
                Email = "anna.verdi@coffeemek.com", 
                Password = "password123",
                CreatedAt = DateTime.UtcNow.AddDays(-8).ToString(),
                UpdatedAt = DateTime.UtcNow.AddDays(-1).ToString(),
            },
            new() { 
                Id = 3, 
                FirstName = "Luca", 
                LastName = "Bianchi", 
                Email = "luca.bianchi@coffeemek.com", 
                Password = "password123",
                CreatedAt = DateTime.UtcNow.AddDays(-5).ToString(),
                UpdatedAt = DateTime.UtcNow.ToString()
            }
        };
    }

    public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
    {
        await Task.Delay(500); // Simula latenza di rete
        
        // Rimuovi password dai risultati
        var usersWithoutPassword = _users.Select(u => new User
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();

        return ApiResponse<List<User>>.SuccessResult(usersWithoutPassword);
    }

    public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
    {
        await Task.Delay(300);
        
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return ApiResponse<User>.ErrorResult("Utente non trovato", 404);
        }

        var userWithoutPassword = new User
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return ApiResponse<User>.SuccessResult(userWithoutPassword);
    }

    public async Task<ApiResponse<User>> CreateUserAsync(User user)
    {
        await Task.Delay(700);

        if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return ApiResponse<User>.ErrorResult("Email già esistente", 409);
        }

        var newUser = new User
        {
            Id = _nextIdCounter++,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = user.Password,
            CreatedAt = DateTime.UtcNow.ToString(),
            UpdatedAt = DateTime.UtcNow.ToString()
        };

        _users.Add(newUser);

        // Restituisci senza password
        var responseUser = new User
        {
            Id = newUser.Id,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email,
            CreatedAt = newUser.CreatedAt,
            UpdatedAt = newUser.UpdatedAt
        };

        return ApiResponse<User>.SuccessResult(responseUser);
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
            return ApiResponse<User>.ErrorResult("Email già esistente", 409);
        }

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.UpdatedAt = DateTime.UtcNow.ToString();
        
        if (!string.IsNullOrEmpty(user.Password))
        {
            existingUser.Password = user.Password;
        }

        var responseUser = new User
        {
            Id = existingUser.Id,
            FirstName = existingUser.FirstName,
            LastName = existingUser.LastName,
            Email = existingUser.Email,
            CreatedAt = existingUser.CreatedAt,
            UpdatedAt = existingUser.UpdatedAt
        };

        return ApiResponse<User>.SuccessResult(responseUser);
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
