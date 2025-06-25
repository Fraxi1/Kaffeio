using System.Collections.Concurrent;
using System.Text.Json;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public class InMemoryTokenService : ITokenService
{
    private readonly ConcurrentDictionary<string, string> _tokens = new();
    private readonly ConcurrentDictionary<string, User> _users = new();
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<InMemoryTokenService> _logger;

    // Usa un identificatore di connessione per Blazor Server
    private string GetConnectionId()
    {
        // Per Blazor Server, usa un ID basato su thread o context
        return Thread.CurrentThread.ManagedThreadId.ToString();
    }

    public InMemoryTokenService(ILogger<InMemoryTokenService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public Task<string?> GetTokenAsync()
    {
        var connectionId = GetConnectionId();
        _tokens.TryGetValue(connectionId, out var token);
        _logger.LogDebug("Retrieved token for connection {ConnectionId}: {HasToken}", connectionId, !string.IsNullOrEmpty(token));
        return Task.FromResult(token);
    }

    public Task SetTokenAsync(string token)
    {
        var connectionId = GetConnectionId();
        _tokens[connectionId] = token;
        _logger.LogDebug("Set token for connection {ConnectionId}", connectionId);
        return Task.CompletedTask;
    }

    public Task RemoveTokenAsync()
    {
        var connectionId = GetConnectionId();
        _tokens.TryRemove(connectionId, out _);
        _logger.LogDebug("Removed token for connection {ConnectionId}", connectionId);
        return Task.CompletedTask;
    }

    public async Task<bool> HasTokenAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public Task SetUserAsync(User user)
    {
        var connectionId = GetConnectionId();
        _users[connectionId] = user;
        _logger.LogDebug("Set user for connection {ConnectionId}: {Email}", connectionId, user.Email);
        return Task.CompletedTask;
    }

    public Task<User?> GetUserAsync()
    {
        var connectionId = GetConnectionId();
        _users.TryGetValue(connectionId, out var user);
        _logger.LogDebug("Retrieved user for connection {ConnectionId}: {Email}", connectionId, user?.Email);
        return Task.FromResult(user);
    }

    public Task RemoveUserAsync()
    {
        var connectionId = GetConnectionId();
        _users.TryRemove(connectionId, out _);
        _logger.LogDebug("Removed user for connection {ConnectionId}", connectionId);
        return Task.CompletedTask;
    }
}
