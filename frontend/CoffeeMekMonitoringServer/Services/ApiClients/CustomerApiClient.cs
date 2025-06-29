using System.Text.Json;
using System.Text;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class CustomerApiClient : ICustomerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CustomerApiClient(HttpClient httpClient, ILogger<CustomerApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<ApiResponse<List<Customer>>> GetAllCustomersAsync()
    {
        try
        {
            _logger.LogDebug("Fetching all customers from CoffeeMek API");
            
            var response = await _httpClient.GetAsync("api/customers");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("API Response - Status: {StatusCode}, Content: {Content}", 
                response.StatusCode, content[..Math.Min(500, content.Length)]);

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<List<Customer>>>(content, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully fetched {Count} customers from CoffeeMek API", wrapper.Data.Count);
                    return ApiResponse<List<Customer>>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    _logger.LogError("API returned unsuccessful response: {Error}", errorMsg);
                    return ApiResponse<List<Customer>>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else
            {
                _logger.LogError("Failed to fetch customers. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, content);
                return ApiResponse<List<Customer>>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while fetching customers from CoffeeMek cloud");
            return ApiResponse<List<Customer>>.ErrorResult("Errore di connessione al cloud CoffeeMek. Verificare la connettività di rete.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error for customers response");
            return ApiResponse<List<Customer>>.ErrorResult("Errore nel formato dei dati ricevuti dall'API CoffeeMek");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout while fetching customers");
            return ApiResponse<List<Customer>>.ErrorResult("Timeout nella connessione all'API CoffeeMek");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching customers");
            return ApiResponse<List<Customer>>.ErrorResult($"Errore imprevisto: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Customer>> GetCustomerByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching customer {CustomerId} from CoffeeMek API", id);
            
            var response = await _httpClient.GetAsync($"api/customers/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<Customer>>(content, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully fetched customer {CustomerId}: {CustomerName}", id, wrapper.Data.Name);
                    return ApiResponse<Customer>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    return ApiResponse<Customer>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Customer {CustomerId} not found", id);
                return ApiResponse<Customer>.ErrorResult("Cliente CoffeeMek non trovato", 404);
            }
            else
            {
                _logger.LogError("Failed to fetch customer {CustomerId}. Status: {StatusCode}", id, response.StatusCode);
                return ApiResponse<Customer>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching customer {CustomerId}", id);
            return ApiResponse<Customer>.ErrorResult($"Errore nel recupero cliente: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Customer>> CreateCustomerAsync(Customer customer)
    {
        try
        {
            _logger.LogDebug("Creating new CoffeeMek customer: {CustomerName}", customer.Name);
            
            // Crea DTO per la creazione (senza Orders nested)
            var createDto = new
            {
                name = customer.Name,
                email = customer.Email,
                phone = customer.Phone,
                address = customer.Address,
                country = customer.Country
            };
            
            var json = JsonSerializer.Serialize(createDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("Sending POST request with payload: {Json}", json);
            
            var response = await _httpClient.PostAsync("api/customers", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Create customer response - Status: {StatusCode}, Content: {Content}", 
                response.StatusCode, responseContent);

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<Customer>>(responseContent, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully created CoffeeMek customer: {CustomerName} with ID: {CustomerId}", 
                        wrapper.Data.Name, wrapper.Data.Id);
                    return ApiResponse<Customer>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    return ApiResponse<Customer>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else
            {
                _logger.LogError("Failed to create customer. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, responseContent);
                
                // Tenta di parsare l'errore dalla risposta
                try
                {
                    var errorWrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<object>>(responseContent, _jsonOptions);
                    var errorMsg = errorWrapper?.Error ?? errorWrapper?.Message ?? response.ReasonPhrase;
                    return ApiResponse<Customer>.ErrorResult($"Errore creazione cliente: {errorMsg}");
                }
                catch
                {
                    return ApiResponse<Customer>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer: {CustomerName}", customer.Name);
            return ApiResponse<Customer>.ErrorResult($"Errore nella creazione del cliente: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Customer>> UpdateCustomerAsync(int id, Customer customer)
    {
        try
        {
            _logger.LogDebug("Updating CoffeeMek customer {CustomerId}: {CustomerName}", id, customer.Name);
            
            // Crea DTO per l'aggiornamento (senza Orders nested)
            var updateDto = new
            {
                name = customer.Name,
                email = customer.Email,
                phone = customer.Phone,
                address = customer.Address,
                country = customer.Country
            };
            
            var json = JsonSerializer.Serialize(updateDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/customers/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<Customer>>(responseContent, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully updated CoffeeMek customer {CustomerId}", id);
                    return ApiResponse<Customer>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    return ApiResponse<Customer>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ApiResponse<Customer>.ErrorResult("Cliente CoffeeMek non trovato", 404);
            }
            else
            {
                try
                {
                    var errorWrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<object>>(responseContent, _jsonOptions);
                    var errorMsg = errorWrapper?.Error ?? errorWrapper?.Message ?? response.ReasonPhrase;
                    return ApiResponse<Customer>.ErrorResult($"Errore aggiornamento: {errorMsg}");
                }
                catch
                {
                    return ApiResponse<Customer>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer {CustomerId}", id);
            return ApiResponse<Customer>.ErrorResult($"Errore nell'aggiornamento del cliente: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAsync(int id)
    {
        try
        {
            _logger.LogDebug("Deleting CoffeeMek customer {CustomerId}", id);
            
            var response = await _httpClient.DeleteAsync($"api/customers/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Per la cancellazione, l'API potrebbe restituire solo un success boolean
                try
                {
                    var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<object>>(responseContent, _jsonOptions);
                    if (wrapper != null && wrapper.Success)
                    {
                        _logger.LogInformation("Successfully deleted CoffeeMek customer {CustomerId}", id);
                        return ApiResponse<bool>.SuccessResult(true);
                    }
                    else
                    {
                        var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Deletion failed";
                        return ApiResponse<bool>.ErrorResult($"Errore cancellazione: {errorMsg}");
                    }
                }
                catch (JsonException)
                {
                    // Se la risposta non è JSON, considera il successo HTTP come successo dell'operazione
                    _logger.LogInformation("Successfully deleted CoffeeMek customer {CustomerId} (non-JSON response)", id);
                    return ApiResponse<bool>.SuccessResult(true);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Customer {CustomerId} not found for deletion", id);
                return ApiResponse<bool>.ErrorResult("Cliente CoffeeMek non trovato", 404);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogWarning("Cannot delete customer {CustomerId} - has dependent records", id);
                return ApiResponse<bool>.ErrorResult("Impossibile eliminare il cliente: ha commesse associate", 409);
            }
            else
            {
                try
                {
                    var errorWrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<object>>(responseContent, _jsonOptions);
                    var errorMsg = errorWrapper?.Error ?? errorWrapper?.Message ?? response.ReasonPhrase;
                    return ApiResponse<bool>.ErrorResult($"Errore cancellazione: {errorMsg}");
                }
                catch
                {
                    return ApiResponse<bool>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
            return ApiResponse<bool>.ErrorResult($"Errore nella cancellazione del cliente: {ex.Message}");
        }
    }
}
