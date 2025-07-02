using System.Text.Json;
using System.Text;
using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class OrderApiClient : IOrderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public OrderApiClient(HttpClient httpClient, ILogger<OrderApiClient> logger)
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

    public async Task<ApiResponse<List<Order>>> GetAllOrdersAsync()
    {
        try
        {
            _logger.LogDebug("Fetching all CoffeeMek orders from production API");
            
            var response = await _httpClient.GetAsync("api/orders");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Orders API Response - Status: {StatusCode}, Content: {Content}", 
                response.StatusCode, content[..Math.Min(500, content.Length)]);

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<List<Order>>>(content, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully fetched {Count} CoffeeMek orders from production API", wrapper.Data.Count);
                    
                    // Enrich orders with calculated properties for compatibility
                    foreach (var order in wrapper.Data)
                    {
                        EnrichOrderForCompatibility(order);
                    }
                    
                    return ApiResponse<List<Order>>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    _logger.LogError("CoffeeMek Orders API returned unsuccessful response: {Error}", errorMsg);
                    return ApiResponse<List<Order>>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else
            {
                _logger.LogError("Failed to fetch CoffeeMek orders. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, content);
                return ApiResponse<List<Order>>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while fetching CoffeeMek orders from cloud");
            return ApiResponse<List<Order>>.ErrorResult("Errore di connessione al cloud CoffeeMek. Verificare la connettività di rete.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error for CoffeeMek orders response");
            return ApiResponse<List<Order>>.ErrorResult("Errore nel formato dei dati ricevuti dall'API CoffeeMek");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout while fetching CoffeeMek orders");
            return ApiResponse<List<Order>>.ErrorResult("Timeout nella connessione all'API CoffeeMek");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching CoffeeMek orders");
            return ApiResponse<List<Order>>.ErrorResult($"Errore imprevisto: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Order>> GetOrderByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching CoffeeMek order {OrderId} from production API", id);
            
            var response = await _httpClient.GetAsync($"api/orders/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<Order>>(content, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully fetched CoffeeMek order {OrderId}: {OrderNumber}", 
                        id, wrapper.Data.OrderNumber);
                    
                    EnrichOrderForCompatibility(wrapper.Data);
                    return ApiResponse<Order>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    return ApiResponse<Order>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("CoffeeMek order {OrderId} not found", id);
                return ApiResponse<Order>.ErrorResult("Commessa CoffeeMek non trovata", 404);
            }
            else
            {
                _logger.LogError("Failed to fetch CoffeeMek order {OrderId}. Status: {StatusCode}", id, response.StatusCode);
                return ApiResponse<Order>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching CoffeeMek order {OrderId}", id);
            return ApiResponse<Order>.ErrorResult($"Errore nel recupero commessa: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Order>>> GetOrdersByCustomerAsync(int customerId)
    {
        try
        {
            _logger.LogDebug("Fetching CoffeeMek orders for customer {CustomerId}", customerId);
            
            var response = await _httpClient.GetAsync($"api/orders/customer/{customerId}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<List<Order>>>(content, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully fetched {Count} CoffeeMek orders for customer {CustomerId}", 
                        wrapper.Data.Count, customerId);
                    
                    foreach (var order in wrapper.Data)
                    {
                        EnrichOrderForCompatibility(order);
                    }
                    
                    return ApiResponse<List<Order>>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    return ApiResponse<List<Order>>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else
            {
                return ApiResponse<List<Order>>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching CoffeeMek orders for customer {CustomerId}", customerId);
            return ApiResponse<List<Order>>.ErrorResult($"Errore nel recupero commesse cliente: {ex.Message}");
        }
    }

  public async Task<ApiResponse<List<Order>>> GetOrdersByStatusAsync(string status)
{
    try
    {
        _logger.LogDebug("Fetching CoffeeMek orders by status: {Status}", status);
        
        var response = await _httpClient.GetAsync($"api/orders/status/{status}");
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<List<Order>>>(content, _jsonOptions);
            
            if (wrapper != null && wrapper.Success && wrapper.Data != null)
            {
                _logger.LogInformation("Successfully fetched {Count} CoffeeMek orders with status: {Status}", 
                    wrapper.Data.Count, status);
                
                foreach (var order in wrapper.Data)
                {
                    EnrichOrderForCompatibility(order);
                }
                
                return ApiResponse<List<Order>>.SuccessResult(wrapper.Data);
            }
            else
            {
                var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                return ApiResponse<List<Order>>.ErrorResult($"API Error: {errorMsg}");
            }
        }
        else
        {
            _logger.LogError("Failed to fetch CoffeeMek orders by status {Status}. Status: {StatusCode}", 
                status, response.StatusCode);
            return ApiResponse<List<Order>>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
        }
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Network error while fetching CoffeeMek orders by status {Status}", status);
        return ApiResponse<List<Order>>.ErrorResult("Errore di connessione al cloud CoffeeMek");
    }
    catch (JsonException ex)
    {
        _logger.LogError(ex, "JSON parsing error for CoffeeMek orders by status {Status}", status);
        return ApiResponse<List<Order>>.ErrorResult("Errore nel formato dei dati ricevuti dall'API CoffeeMek");
    }
    catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
    {
        _logger.LogError(ex, "Timeout while fetching CoffeeMek orders by status {Status}", status);
        return ApiResponse<List<Order>>.ErrorResult("Timeout nella connessione all'API CoffeeMek");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error while fetching CoffeeMek orders by status {Status}", status);
        return ApiResponse<List<Order>>.ErrorResult($"Errore imprevisto: {ex.Message}");
    }
}

    public async Task<ApiResponse<Order>> CreateOrderAsync(Order order)
    {
        try
        {
            _logger.LogDebug("Creating new CoffeeMek order: {OrderNumber} for customer {CustomerId}", 
                order.OrderNumber, order.CustomerId);
            
            // Crea DTO per la creazione (senza nested objects e proprietà read-only)
            var createDto = new
            {
                customerId = order.CustomerId,
                quantity = order.Quantity,
                orderDate = order.OrderDate,
                deliveryDate = order.DeliveryDate,
                status = order.Status,
                notes = order.Notes
            };
            
            var json = JsonSerializer.Serialize(createDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("Sending CoffeeMek order creation request with payload: {Json}", json);
            
            var response = await _httpClient.PostAsync("api/orders", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("Create CoffeeMek order response - Status: {StatusCode}, Content: {Content}", 
                response.StatusCode, responseContent);

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<Order>>(responseContent, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully created CoffeeMek order: {OrderNumber} with ID: {OrderId}", 
                        wrapper.Data.OrderNumber, wrapper.Data.Id);
                    
                    EnrichOrderForCompatibility(wrapper.Data);
                    return ApiResponse<Order>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    return ApiResponse<Order>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else
            {
                _logger.LogError("Failed to create CoffeeMek order. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, responseContent);
                
                try
                {
                    var errorWrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<object>>(responseContent, _jsonOptions);
                    var errorMsg = errorWrapper?.Error ?? errorWrapper?.Message ?? response.ReasonPhrase;
                    return ApiResponse<Order>.ErrorResult($"Errore creazione commessa CoffeeMek: {errorMsg}");
                }
                catch
                {
                    return ApiResponse<Order>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating CoffeeMek order for customer {CustomerId}", order.CustomerId);
            return ApiResponse<Order>.ErrorResult($"Errore nella creazione della commessa: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Order>> UpdateOrderAsync(int id, Order order)
    {
        try
        {
            _logger.LogDebug("Updating CoffeeMek order {OrderId}: {OrderNumber}", id, order.OrderNumber);
            
            // Crea DTO per l'aggiornamento
            var updateDto = new
            {
                customerId = order.CustomerId,
                quantity = order.Quantity,
                orderDate = order.OrderDate,
                deliveryDate = order.DeliveryDate,
                status = order.Status,
                notes = order.Notes
            };
            
            var json = JsonSerializer.Serialize(updateDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/orders/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<Order>>(responseContent, _jsonOptions);
                
                if (wrapper != null && wrapper.Success && wrapper.Data != null)
                {
                    _logger.LogInformation("Successfully updated CoffeeMek order {OrderId}", id);
                    
                    EnrichOrderForCompatibility(wrapper.Data);
                    return ApiResponse<Order>.SuccessResult(wrapper.Data);
                }
                else
                {
                    var errorMsg = wrapper?.Error ?? wrapper?.Message ?? "Invalid response structure";
                    return ApiResponse<Order>.ErrorResult($"API Error: {errorMsg}");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ApiResponse<Order>.ErrorResult("Commessa CoffeeMek non trovata", 404);
            }
            else
            {
                try
                {
                    var errorWrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<object>>(responseContent, _jsonOptions);
                    var errorMsg = errorWrapper?.Error ?? errorWrapper?.Message ?? response.ReasonPhrase;
                    return ApiResponse<Order>.ErrorResult($"Errore aggiornamento: {errorMsg}");
                }
                catch
                {
                    return ApiResponse<Order>.ErrorResult($"HTTP Error {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating CoffeeMek order {OrderId}", id);
            return ApiResponse<Order>.ErrorResult($"Errore nell'aggiornamento della commessa: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
    {
        try
        {
            _logger.LogDebug("Deleting CoffeeMek order {OrderId}", id);
            
            var response = await _httpClient.DeleteAsync($"api/orders/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var wrapper = JsonSerializer.Deserialize<ApiResponse<object>.ApiResponseWrapper<object>>(responseContent, _jsonOptions);
                    if (wrapper != null && wrapper.Success)
                    {
                        _logger.LogInformation("Successfully deleted CoffeeMek order {OrderId}", id);
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
                    _logger.LogInformation("Successfully deleted CoffeeMek order {OrderId} (non-JSON response)", id);
                    return ApiResponse<bool>.SuccessResult(true);
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("CoffeeMek order {OrderId} not found for deletion", id);
                return ApiResponse<bool>.ErrorResult("Commessa CoffeeMek non trovata", 404);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogWarning("Cannot delete CoffeeMek order {OrderId} - has dependent records", id);
                return ApiResponse<bool>.ErrorResult("Impossibile eliminare la commessa: ha lotti di produzione associati", 409);
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
            _logger.LogError(ex, "Error deleting CoffeeMek order {OrderId}", id);
            return ApiResponse<bool>.ErrorResult($"Errore nella cancellazione della commessa: {ex.Message}");
        }
    }

    /// <summary>
    /// Arricchisce l'order con proprietà per compatibilità con il frontend esistente
    /// </summary>
    private void EnrichOrderForCompatibility(Order order)
    {
        try
        {
            // Imposta proprietà di compatibilità
            order.Priority = "Medium"; // Default priority
            order.Description = order.Notes ?? $"Commessa {order.OrderNumber} - {order.Quantity} macchine da caffè CoffeeMek";
            
            // Stima facility basata sul paese del cliente (logica semplificata)
            if (order.Customer != null)
            {
                order.FacilityId = EstimateFacilityFromCustomer(order.Customer);
                order.Facility = CreateFacilityForCompatibility(order.FacilityId);
            }
            
            _logger.LogDebug("Enriched CoffeeMek order {OrderId} for frontend compatibility", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error enriching CoffeeMek order {OrderId} for compatibility", order.Id);
        }
    }

    private int EstimateFacilityFromCustomer(OrderCustomer customer)
    {
        // Stima sede produttiva basata sul paese del cliente
        return customer.Country?.ToLower() switch
        {
            "italy" => 1, // Milano
            "brasil" => 2, // São Paulo
            "vietnam" => 3, // Ho Chi Minh
            _ => 1 // Default a Milano
        };
    }

    private Facility CreateFacilityForCompatibility(int facilityId)
    {
        return facilityId switch
        {
            1 => new Facility { Id = 1, Name = "Stabilimento Milano", Location = "Italy", TimeZone = "+1" },
            2 => new Facility { Id = 2, Name = "Fábrica São Paulo", Location = "Brasil", TimeZone = "-3" },
            3 => new Facility { Id = 3, Name = "CoffeeMek Ho Chi Minh Factory", Location = "Vietnam", TimeZone = "+7" },
            _ => new Facility { Id = 1, Name = "Stabilimento Milano", Location = "Italy", TimeZone = "+1" }
        };
    }
}
