using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<CustomAuthenticationStateProvider> _logger;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ITokenService tokenService, ILogger<CustomAuthenticationStateProvider> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            _logger.LogDebug("GetAuthenticationStateAsync called");
            
            var token = await _tokenService.GetTokenAsync();
            _logger.LogDebug("Token retrieved: {HasToken}", !string.IsNullOrEmpty(token));
            
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("No token found, returning anonymous user");
                return new AuthenticationState(_anonymous);
            }

            var user = await _tokenService.GetUserAsync();
            _logger.LogDebug("User retrieved: {Email}", user?.Email ?? "(null)");
            
            if (user == null)
            {
                _logger.LogDebug("No user found, returning anonymous user");
                return new AuthenticationState(_anonymous);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id?.ToString() ?? ""),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("UserId", user.Id?.ToString() ?? "")
            };

            var identity = new ClaimsIdentity(claims, "kaffeio-jwt");
            var principal = new ClaimsPrincipal(identity);

            _logger.LogInformation("User authenticated successfully: {Email}, IsAuthenticated: {IsAuthenticated}", 
                user.Email, identity.IsAuthenticated);
            
            return new AuthenticationState(principal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task MarkUserAsAuthenticated(User user, string token)
    {
        _logger.LogInformation("MarkUserAsAuthenticated called for user: {Email}", user.Email);
        
        // Salva token e user
        await _tokenService.SetTokenAsync(token);
        await _tokenService.SetUserAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id?.ToString() ?? ""),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("UserId", user.Id?.ToString() ?? "")
        };

        var identity = new ClaimsIdentity(claims, "kaffeio-jwt");
        var principal = new ClaimsPrincipal(identity);

        _logger.LogInformation("Notifying authentication state changed for user: {Email}", user.Email);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        _logger.LogInformation("MarkUserAsLoggedOut called");
        
        await _tokenService.RemoveTokenAsync();
        await _tokenService.RemoveUserAsync();
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}
