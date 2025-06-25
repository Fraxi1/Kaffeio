using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class UserApiClient : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UserApiClient> _logger;

    public UserApiClient(
        IHttpClientFactory httpClientFactory, 
        ITokenService tokenService,
        ILogger<UserApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("CoffeeMekApi");
        _tokenService = tokenService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<bool> AddJwtHeaderAsync()
    {
        var token = await _tokenService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
            return true;
        }
        return false;
    }

    public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync("api/users");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("GetAllUsers response: Status={Status}, Content={Content}", 
                response.StatusCode, content);

            if (response.IsSuccessStatusCode)
            {
                // Deserializza secondo la struttura JSON reale con wrapper
                var apiResponse = JsonSerializer.Deserialize<UsersListResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<List<User>>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    _logger.LogWarning("API returned success=false or null data");
                    return ApiResponse<List<User>>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<List<User>>.ErrorResult("Token scaduto. Effettua nuovamente il login.", 401);
            }

            return ApiResponse<List<User>>.ErrorResult(
                $"Errore nel caricamento utenti: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error in GetAllUsers");
            return ApiResponse<List<User>>.ErrorResult("Errore di deserializzazione della risposta");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error in GetAllUsers");
            return ApiResponse<List<User>>.ErrorResult("Errore di connessione all'API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetAllUsers");
            return ApiResponse<List<User>>.ErrorResult($"Errore imprevisto: {ex.Message}");
        }
    }

    public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync($"api/users/{id}");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("GetUserById response: Status={Status}, Content={Content}", 
                response.StatusCode, content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<UserResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<User>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<User>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiResponse<User>.ErrorResult("Utente non trovato", 404);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<User>.ErrorResult("Token scaduto", 401);
            }

            return ApiResponse<User>.ErrorResult(
                $"Errore nel caricamento utente: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error in GetUserById for id {Id}", id);
            return ApiResponse<User>.ErrorResult("Errore di deserializzazione della risposta");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserById for id {Id}", id);
            return ApiResponse<User>.ErrorResult($"Errore: {ex.Message}");
        }
    }

    public async Task<ApiResponse<User>> CreateUserAsync(User user)
    {
        try
        {
            await AddJwtHeaderAsync();
            
            // Payload secondo la documentazione API Kaffeio
            var createRequest = new
            {
                email = user.Email,
                password = user.Password,
                firstName = user.FirstName,
                lastName = user.LastName
            };

            var json = JsonSerializer.Serialize(createRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/users", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("CreateUser response: Status={Status}, Content={Content}", 
                response.StatusCode, responseContent);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<UserCreateResponse>(responseContent, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<User>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<User>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return ApiResponse<User>.ErrorResult("Email già in uso", 409);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<User>.ErrorResult("Token scaduto", 401);
            }

            return ApiResponse<User>.ErrorResult(
                $"Errore nella creazione: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return ApiResponse<User>.ErrorResult($"Errore: {ex.Message}");
        }
    }

    public async Task<ApiResponse<User>> UpdateUserAsync(int id, User user)
    {
        try
        {
            await AddJwtHeaderAsync();
            
            // Prepara payload per aggiornamento (tutti i campi opzionali)
            var updateRequest = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(user.Email)) updateRequest["email"] = user.Email;
            if (!string.IsNullOrEmpty(user.FirstName)) updateRequest["firstName"] = user.FirstName;
            if (!string.IsNullOrEmpty(user.LastName)) updateRequest["lastName"] = user.LastName;
            if (!string.IsNullOrEmpty(user.Password)) updateRequest["password"] = user.Password;

            var json = JsonSerializer.Serialize(updateRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/users/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<UserResponse>(responseContent, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<User>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<User>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiResponse<User>.ErrorResult("Utente non trovato", 404);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<User>.ErrorResult("Token scaduto", 401);
            }

            return ApiResponse<User>.ErrorResult(
                $"Errore nell'aggiornamento: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {Id}", id);
            return ApiResponse<User>.ErrorResult($"Errore: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.DeleteAsync($"api/users/{id}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<UserDeleteResponse>(responseContent, _jsonOptions);
                
                if (apiResponse?.Success == true)
                {
                    return ApiResponse<bool>.SuccessResult(true);
                }
                else
                {
                    return ApiResponse<bool>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiResponse<bool>.ErrorResult("Utente non trovato", 404);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<bool>.ErrorResult("Token scaduto", 401);
            }

            return ApiResponse<bool>.ErrorResult(
                $"Errore nell'eliminazione: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            return ApiResponse<bool>.ErrorResult($"Errore: {ex.Message}");
        }
    }
}
