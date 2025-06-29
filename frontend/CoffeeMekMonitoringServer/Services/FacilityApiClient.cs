using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class FacilityApiClient : IFacilityService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITokenService _tokenService;
    private readonly ILogger<FacilityApiClient> _logger;

    public FacilityApiClient(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        ILogger<FacilityApiClient> logger)
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

    public async Task<ApiResponse<List<Facility>>> GetAllFacilitiesAsync()
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync("api/facilities");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("GetAllFacilities response: Status={Status}, Content={Content}", 
                response.StatusCode, content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<FacilitiesListResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<List<Facility>>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<List<Facility>>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<List<Facility>>.ErrorResult("Token scaduto. Effettua nuovamente il login.", 401);
            }

            return ApiResponse<List<Facility>>.ErrorResult(
                $"Errore nel caricamento facilities: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllFacilities");
            return ApiResponse<List<Facility>>.ErrorResult($"Errore: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Facility>> GetFacilityByIdAsync(int id)
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync($"api/facilities/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<FacilityResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<Facility>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<Facility>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiResponse<Facility>.ErrorResult("Facility non trovata", 404);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<Facility>.ErrorResult("Token scaduto", 401);
            }

            return ApiResponse<Facility>.ErrorResult(
                $"Errore nel caricamento facility: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetFacilityById for id {Id}", id);
            return ApiResponse<Facility>.ErrorResult($"Errore: {ex.Message}");
        }
    }
}
