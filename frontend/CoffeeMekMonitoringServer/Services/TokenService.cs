using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;
    private const string TokenKey = "kaffeio_jwt_token";
    private const string UserKey = "kaffeio_user_data";

    public TokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public Task<string?> GetTokenAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        var token = session?.GetString(TokenKey);
        return Task.FromResult(token);
    }

    public Task SetTokenAsync(string token)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.SetString(TokenKey, token);
        return Task.CompletedTask;
    }

    public Task RemoveTokenAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public async Task<bool> HasTokenAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public Task SetUserAsync(User user)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        var userJson = JsonSerializer.Serialize(user, _jsonOptions);
        session?.SetString(UserKey, userJson);
        return Task.CompletedTask;
    }

    public Task<User?> GetUserAsync()
    {
        try
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var userJson = session?.GetString(UserKey);
            if (string.IsNullOrEmpty(userJson))
                return Task.FromResult<User?>(null);

            var user = JsonSerializer.Deserialize<User>(userJson, _jsonOptions);
            return Task.FromResult(user);
        }
        catch
        {
            return Task.FromResult<User?>(null);
        }
    }

    public Task RemoveUserAsync()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.Remove(UserKey);
        return Task.CompletedTask;
    }
}
