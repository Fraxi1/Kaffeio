using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace CoffeeMekMonitoringServer.Models;

public class Order
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("customerId")]
    [Required(ErrorMessage = "Il cliente è obbligatorio")]
    public int CustomerId { get; set; }
    
    [JsonPropertyName("quantity")]
    [Required(ErrorMessage = "La quantità è obbligatoria")]
    [Range(1, 10000, ErrorMessage = "La quantità deve essere tra 1 e 10000")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }
    
    [JsonPropertyName("deliveryDate")]
    public DateTime? DeliveryDate { get; set; }
    
    [JsonPropertyName("status")]
    [Required(ErrorMessage = "Lo stato è obbligatorio")]
    public string Status { get; set; } = "Pending";
    
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
    
    // Nested Objects dalla API
    [JsonPropertyName("customer")]
    public OrderCustomer? Customer { get; set; }
    
    [JsonPropertyName("lots")]
    public List<OrderLot> Lots { get; set; } = new();
    
    // Proprietà aggiuntive per compatibilità con il frontend esistente
    [JsonIgnore]
    public int FacilityId { get; set; } // Per compatibilità, potrebbe essere derivato dal customer
    
    [JsonIgnore]
    public string Priority { get; set; } = "Medium"; // Per compatibilità
    
    [JsonIgnore]
    public string Description { get; set; } = string.Empty; // Per compatibilità
    
    [JsonIgnore]
    public Facility? Facility { get; set; } // Per compatibilità
    
    // Proprietà calcolate per UI CoffeeMek
    [JsonIgnore]
    public string StatusBadgeColor => Status?.ToLower() switch
    {
        "pending" => "warning",
        "inprogress" => "primary",
        "completed" => "success",
        "cancelled" => "danger",
        _ => "secondary"
    };
    
    [JsonIgnore]
    public string StatusIcon => Status?.ToLower() switch
    {
        "pending" => "fas fa-clock",
        "inprogress" => "fas fa-cog fa-spin",
        "completed" => "fas fa-check-circle",
        "cancelled" => "fas fa-times-circle",
        _ => "fas fa-question-circle"
    };
    
    [JsonIgnore]
    public bool IsOverdue => DeliveryDate.HasValue && DeliveryDate.Value < DateTime.Now && Status != "Completed";
    
    [JsonIgnore]
    public int TotalLots => Lots?.Count ?? 0;
    
    [JsonIgnore]
    public int CompletedLots => Lots?.Count(l => l.Status == "completed") ?? 0;
    
    [JsonIgnore]
    public double CompletionPercentage => TotalLots > 0 ? Math.Round((double)CompletedLots / TotalLots * 100, 1) : 0;
}

/// <summary>
/// Customer nested nell'order secondo la struttura API reale
/// </summary>
public class OrderCustomer
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
    
    // Proprietà calcolata per UI
    [JsonIgnore]
    public string CountryFlag => GetCountryFlag(Country);
    
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
/// Lotto nested nell'order
/// </summary>
public class OrderLot
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("currentPhase")]
    public string? CurrentPhase { get; set; } // Fresa, Tornio, Assemblaggio, Test
    
    [JsonPropertyName("progress")]
    public int Progress { get; set; } // 0-100%
}
