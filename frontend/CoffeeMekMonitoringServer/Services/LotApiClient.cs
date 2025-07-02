using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class LotApiClient : ILotService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LotApiClient> _logger;

    public LotApiClient(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        ILogger<LotApiClient> logger)
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

    public async Task<ApiResponse<List<Lot>>> GetAllLotsAsync()
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync("api/lots");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("GetAllLots response: Status={Status}, Content={Content}", 
                response.StatusCode, content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<LotsListResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<List<Lot>>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    _logger.LogWarning("API returned success=false or null data for lots");
                    return ApiResponse<List<Lot>>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<List<Lot>>.ErrorResult("Token scaduto. Effettua nuovamente il login.", 401);
            }

            return ApiResponse<List<Lot>>.ErrorResult(
                $"Errore nel caricamento lotti: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error in GetAllLots");
            return ApiResponse<List<Lot>>.ErrorResult("Errore di deserializzazione della risposta");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error in GetAllLots");
            return ApiResponse<List<Lot>>.ErrorResult("Errore di connessione all'API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetAllLots");
            return ApiResponse<List<Lot>>.ErrorResult($"Errore imprevisto: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Lot>> GetLotByIdAsync(int id)
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync($"api/lots/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<LotResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<Lot>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<Lot>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiResponse<Lot>.ErrorResult("Lotto non trovato", 404);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<Lot>.ErrorResult("Token scaduto", 401);
            }

            return ApiResponse<Lot>.ErrorResult(
                $"Errore nel caricamento lotto: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetLotById for id {Id}", id);
            return ApiResponse<Lot>.ErrorResult($"Errore: {ex.Message}");
        }
    }
}
