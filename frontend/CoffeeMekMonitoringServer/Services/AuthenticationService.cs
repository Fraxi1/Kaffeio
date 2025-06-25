using System.Text;
using System.Text.Json;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationService(
        IHttpClientFactory httpClientFactory, 
        ITokenService tokenService,
        ILogger<AuthenticationService> logger,
        IServiceProvider serviceProvider)
    {
        _httpClient = httpClientFactory.CreateClient("CoffeeMekApi");
        _tokenService = tokenService;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new
            {
                email = email,
                password = password
            };

            var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Attempting login for user: {Email} to Kaffeio API", email);
            
            var response = await _httpClient.PostAsync("api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Login response: Status={Status}, Content={Content}", 
                response.StatusCode, responseContent);

            if (response.IsSuccessStatusCode)
            {
                // Parsing secondo la struttura JSON reale mostrata
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent, _jsonOptions);
                
                // Controlla se c'è la proprietà "success" e "data"
                if (apiResponse.TryGetProperty("success", out var successElement) && 
                    successElement.GetBoolean() && 
                    apiResponse.TryGetProperty("data", out var dataElement))
                {
                    var loginResponse = new LoginResponse
                    {
                        AccessToken = dataElement.GetProperty("access_token").GetString() ?? string.Empty,
                        User = JsonSerializer.Deserialize<User>(
                            dataElement.GetProperty("user").GetRawText(), 
                            _jsonOptions) ?? new User()
                    };

                    if (!string.IsNullOrEmpty(loginResponse.AccessToken))
                    {
                        // Salva in memoria invece che in sessione
                        await _tokenService.SetTokenAsync(loginResponse.AccessToken);
                        await _tokenService.SetUserAsync(loginResponse.User);
                        
                        await NotifyAuthenticationStateChanged(loginResponse.User, loginResponse.AccessToken);
                        
                        _logger.LogInformation("Login successful for user: {Email}", email);
                        return ApiResponse<LoginResponse>.SuccessResult(loginResponse);
                    }
                }
                else
                {
                    // Fallback: prova la struttura originale della documentazione
                    if (apiResponse.TryGetProperty("access_token", out var tokenElement) &&
                        apiResponse.TryGetProperty("user", out var userElement))
                    {
                        var loginResponse = new LoginResponse
                        {
                            AccessToken = tokenElement.GetString() ?? string.Empty,
                            User = JsonSerializer.Deserialize<User>(userElement.GetRawText(), _jsonOptions) ?? new User()
                        };

                        if (!string.IsNullOrEmpty(loginResponse.AccessToken))
                        {
                            await _tokenService.SetTokenAsync(loginResponse.AccessToken);
                            await _tokenService.SetUserAsync(loginResponse.User);
                            await NotifyAuthenticationStateChanged(loginResponse.User, loginResponse.AccessToken);
                            
                            _logger.LogInformation("Login successful for user: {Email} (fallback format)", email);
                            return ApiResponse<LoginResponse>.SuccessResult(loginResponse);
                        }
                    }
                }
            }

            _logger.LogWarning("Login failed for user: {Email}. Status: {Status}, Response: {Response}", 
                email, response.StatusCode, responseContent);
            
            var errorMsg = response.StatusCode == System.Net.HttpStatusCode.Unauthorized 
                ? "Credenziali non valide" 
                : $"Errore del server: {response.StatusCode}";

            return ApiResponse<LoginResponse>.ErrorResult(errorMsg, (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error for user: {Email}", email);
            return ApiResponse<LoginResponse>.ErrorResult("Errore nella risposta del server");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error for user: {Email}", email);
            return ApiResponse<LoginResponse>.ErrorResult("Errore di connessione. Verifica che l'API Kaffeio sia attiva");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error for user: {Email}", email);
            return ApiResponse<LoginResponse>.ErrorResult($"Errore imprevisto: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        await _tokenService.RemoveTokenAsync();
        await _tokenService.RemoveUserAsync();
        await NotifyAuthenticationStateChanged(null, null);
        _logger.LogInformation("User logged out");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        return await _tokenService.HasTokenAsync();
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        return await _tokenService.GetUserAsync();
    }

    private async Task NotifyAuthenticationStateChanged(User? user, string? token)
    {
        try
        {
            var authStateProvider = _serviceProvider.GetService<CustomAuthenticationStateProvider>();
            if (authStateProvider != null)
            {
                if (user != null && !string.IsNullOrEmpty(token))
                {
                    await authStateProvider.MarkUserAsAuthenticated(user, token);
                
                    // IMPORTANTE: Forza un refresh dell'interfaccia
                    await Task.Delay(100);
                    await authStateProvider.GetAuthenticationStateAsync();
                }
                else
                {
                    await authStateProvider.MarkUserAsLoggedOut();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying authentication state change");
        }
    }
}
