using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public class AutoLoginService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<AutoLoginService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly User _defaultUser;

    public AutoLoginService(
        ITokenService tokenService,
        ILogger<AutoLoginService> logger,
        IServiceProvider serviceProvider)
    {
        _tokenService = tokenService;
        _logger = logger;
        _serviceProvider = serviceProvider;
        
        // UTENTE DI DEFAULT - Nessun input richiesto
        _defaultUser = new User
        {
            Id = 1,
            FirstName = "Admin CoffeeMek",
            Email = "admin@coffemek.com", 
            CreatedAt = DateTime.UtcNow.AddMonths(-12).ToString()
        };
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("🚀 AUTO-LOGIN AUTOMATICO - Utente predefinito CoffeeMek");
            
            // Simula un piccolo delay per realismo
            await Task.Delay(Random.Shared.Next(100, 300));

            // Genera token per utente di default
            var fakeToken = GenerateDefaultUserToken();
            
            var loginResponse = new LoginResponse
            {
                AccessToken = fakeToken,
                User = _defaultUser
            };

            // Salva automaticamente token e utente
            await _tokenService.SetTokenAsync(fakeToken);
            await _tokenService.SetUserAsync(_defaultUser);
            await NotifyAuthenticationStateChanged(_defaultUser, fakeToken);

           
            return ApiResponse<LoginResponse>.SuccessResult(loginResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Errore nell'auto-login");
            return ApiResponse<LoginResponse>.ErrorResult($"Errore auto-login: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        await _tokenService.RemoveTokenAsync();
        await _tokenService.RemoveUserAsync();
        await NotifyAuthenticationStateChanged(null, null);
        
        _logger.LogInformation("🚪 Logout automatico - Utente: {UserName}", _defaultUser.FirstName);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        return await _tokenService.HasTokenAsync();
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        return await _tokenService.GetUserAsync();
    }

    private string GenerateDefaultUserToken()
    {
        // Header JWT
        var header = Convert.ToBase64String(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new
        {
            alg = "HS256",
            typ = "JWT"
        }));

        // Payload JWT per utente di default
        var payload = Convert.ToBase64String(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new
        {
            sub = _defaultUser.Id.ToString(),
            email = _defaultUser.Email,
            name = _defaultUser.FirstName,
         
            iss = "CoffeeMek-Auto-Login",
            aud = "CoffeeMek-Frontend",
            iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds(), // 24 ore validità
            jti = Guid.NewGuid().ToString(),
            auth_type = "auto-login-default",
            auto_login = true,
            login_time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        }));

        // Signature
        var signature = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"auto_login_{DateTime.UtcNow.Ticks}")
        );

        return $"{header}.{payload}.{signature}";
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
                    await Task.Delay(50);
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
            _logger.LogError(ex, "Errore notifica cambio stato auto-login");
        }
    }
}
