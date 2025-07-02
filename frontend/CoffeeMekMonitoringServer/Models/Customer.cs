using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class Customer
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
    
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
    
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("orders")]
    public List<CustomerOrder> Orders { get; set; } = new();
    
    // Proprietà calcolate per UI
    [JsonIgnore]
    public string CountryFlag => GetCountryFlag(Country);
    
    [JsonIgnore]
    public int TotalOrders => Orders?.Count ?? 0;
    
    [JsonIgnore]
    public int ActiveOrders => Orders?.Count(o => o.Status == "Pending" || o.Status == "InProgress") ?? 0;
    
    private string GetCountryFlag(string country) => country?.ToLower() switch
    {
        "italy" => "🇮🇹",
        "brasil" => "🇧🇷",
        "vietnam" => "🇻🇳",
        "germany" => "🇩🇪",
        "france" => "🇫🇷",
        "usa" => "🇺🇸",
        "spain" => "🇪🇸",
        _ => "🌍"
    };
}

/// <summary>
/// Ordine nested nel customer secondo la struttura API reale
/// </summary>
public class CustomerOrder
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("customerId")]
    public int CustomerId { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }
    
    [JsonPropertyName("deliveryDate")]
    public DateTime? DeliveryDate { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
