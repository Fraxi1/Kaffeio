using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class MachineApiClient : IMachineService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITokenService _tokenService;
    private readonly ILogger<MachineApiClient> _logger;

    public MachineApiClient(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        ILogger<MachineApiClient> logger)
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

    public async Task<ApiResponse<List<Machine>>> GetAllMachinesAsync()
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync("api/machines");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("GetAllMachines response: Status={Status}, Content={Content}", 
                response.StatusCode, content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<MachinesListResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<List<Machine>>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<List<Machine>>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<List<Machine>>.ErrorResult("Token scaduto. Effettua nuovamente il login.", 401);
            }

            return ApiResponse<List<Machine>>.ErrorResult(
                $"Errore nel caricamento macchine: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllMachines");
            return ApiResponse<List<Machine>>.ErrorResult($"Errore: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Machine>> GetMachineByIdAsync(int id)
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync($"api/machines/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<MachineResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<Machine>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<Machine>.ErrorResult("Risposta API non valida");
                }
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ApiResponse<Machine>.ErrorResult("Macchina non trovata", 404);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.RemoveTokenAsync();
                return ApiResponse<Machine>.ErrorResult("Token scaduto", 401);
            }

            return ApiResponse<Machine>.ErrorResult(
                $"Errore nel caricamento macchina: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMachineById for id {Id}", id);
            return ApiResponse<Machine>.ErrorResult($"Errore: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Machine>>> GetMachinesByFacilityAsync(int facilityId)
    {
        try
        {
            await AddJwtHeaderAsync();
            var response = await _httpClient.GetAsync($"api/facilities/{facilityId}/machines");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<MachinesListResponse>(content, _jsonOptions);
                
                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return ApiResponse<List<Machine>>.SuccessResult(apiResponse.Data);
                }
                else
                {
                    return ApiResponse<List<Machine>>.ErrorResult("Risposta API non valida");
                }
            }

            return ApiResponse<List<Machine>>.ErrorResult(
                $"Errore nel caricamento macchine per facility: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMachinesByFacility for facilityId {FacilityId}", facilityId);
            return ApiResponse<List<Machine>>.ErrorResult($"Errore: {ex.Message}");
        }
    }
}
