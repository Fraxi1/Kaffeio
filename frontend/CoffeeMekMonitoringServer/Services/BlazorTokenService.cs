using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public class BlazorTokenService : ITokenService
{
    private static readonly ConcurrentDictionary<string, string> _tokens = new();
    private static readonly ConcurrentDictionary<string, User> _users = new();
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<BlazorTokenService> _logger;
    
    // Usa un approccio più stabile per Blazor Server
    private static string _currentConnectionId = Guid.NewGuid().ToString();

    public BlazorTokenService(ILogger<BlazorTokenService> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    private string GetConnectionId()
    {
        // Per semplicità, usa un ID globale per l'intera applicazione
        // In un'app reale potresti voler usare un approccio più sofisticato
        return "global-session";
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
        _logger.LogInformation("Set token for connection {ConnectionId}", connectionId);
        return Task.CompletedTask;
    }

    public Task RemoveTokenAsync()
    {
        var connectionId = GetConnectionId();
        _tokens.TryRemove(connectionId, out _);
        _logger.LogInformation("Removed token for connection {ConnectionId}", connectionId);
        return Task.CompletedTask;
    }

    public async Task<bool> HasTokenAsync()
    {
        var token = await GetTokenAsync();
        var hasToken = !string.IsNullOrEmpty(token);
        _logger.LogDebug("HasToken check: {HasToken}", hasToken);
        return hasToken;
    }

    public Task SetUserAsync(User user)
    {
        var connectionId = GetConnectionId();
        _users[connectionId] = user;
        _logger.LogInformation("Set user for connection {ConnectionId}: {Email}", connectionId, user.Email);
        return Task.CompletedTask;
    }

    public Task<User?> GetUserAsync()
    {
        var connectionId = GetConnectionId();
        _users.TryGetValue(connectionId, out var user);
        _logger.LogDebug("Retrieved user for connection {ConnectionId}: {Email}", connectionId, user?.Email ?? "(null)");
        return Task.FromResult(user);
    }

    public Task RemoveUserAsync()
    {
        var connectionId = GetConnectionId();
        _users.TryRemove(connectionId, out _);
        _logger.LogInformation("Removed user for connection {ConnectionId}", connectionId);
        return Task.CompletedTask;
    }
}
