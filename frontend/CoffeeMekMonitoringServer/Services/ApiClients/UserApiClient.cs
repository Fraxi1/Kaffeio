using System.Net;
using System.Text;
using System.Text.Json;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class UserApiClient : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CoffeeMekApi");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("users");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var users = JsonSerializer.Deserialize<List<User>>(content, _jsonOptions);
                return ApiResponse<List<User>>.SuccessResult(users ?? new List<User>());
            }

            return ApiResponse<List<User>>.ErrorResult(
                $"Errore nel caricamento utenti: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<List<User>>.ErrorResult($"Errore di connessione: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<List<User>>.ErrorResult("Timeout della richiesta");
        }
        catch (JsonException ex)
        {
            return ApiResponse<List<User>>.ErrorResult($"Errore di deserializzazione: {ex.Message}");
        }
    }

    public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"users/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var user = JsonSerializer.Deserialize<User>(content, _jsonOptions);
                return user != null 
                    ? ApiResponse<User>.SuccessResult(user)
                    : ApiResponse<User>.ErrorResult("Utente non trovato", 404);
            }

            return ApiResponse<User>.ErrorResult(
                $"Errore nel caricamento utente: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<User>.ErrorResult($"Errore di connessione: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return ApiResponse<User>.ErrorResult($"Errore di deserializzazione: {ex.Message}");
        }
    }

    public async Task<ApiResponse<User>> CreateUserAsync(User user)
    {
        try
        {
            var json = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("users", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var createdUser = JsonSerializer.Deserialize<User>(responseContent, _jsonOptions);
                return createdUser != null
                    ? ApiResponse<User>.SuccessResult(createdUser)
                    : ApiResponse<User>.ErrorResult("Errore nella creazione utente");
            }

            return ApiResponse<User>.ErrorResult(
                $"Errore nella creazione: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<User>.ErrorResult($"Errore di connessione: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return ApiResponse<User>.ErrorResult($"Errore di serializzazione: {ex.Message}");
        }
    }

    public async Task<ApiResponse<User>> UpdateUserAsync(int id, User user)
    {
        try
        {
            var json = JsonSerializer.Serialize(user, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"users/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var updatedUser = JsonSerializer.Deserialize<User>(responseContent, _jsonOptions);
                return updatedUser != null
                    ? ApiResponse<User>.SuccessResult(updatedUser)
                    : ApiResponse<User>.ErrorResult("Errore nell'aggiornamento utente");
            }

            return ApiResponse<User>.ErrorResult(
                $"Errore nell'aggiornamento: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<User>.ErrorResult($"Errore di connessione: {ex.Message}");
        }
        catch (JsonException ex)
        {
            return ApiResponse<User>.ErrorResult($"Errore di serializzazione: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"users/{id}");

            if (response.IsSuccessStatusCode)
            {
                return ApiResponse<bool>.SuccessResult(true);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiResponse<bool>.ErrorResult("Utente non trovato", 404);
            }

            return ApiResponse<bool>.ErrorResult(
                $"Errore nell'eliminazione: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            return ApiResponse<bool>.ErrorResult($"Errore di connessione: {ex.Message}");
        }
    }
}
