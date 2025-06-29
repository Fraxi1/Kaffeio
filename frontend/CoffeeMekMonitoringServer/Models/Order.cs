using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class Order
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Il numero ordine è obbligatorio")]
    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;

    [JsonPropertyName("customerId")]
    public int CustomerId { get; set; }

    [JsonPropertyName("customer")]
    public Customer? Customer { get; set; }

    [JsonPropertyName("facilityId")]
    public int FacilityId { get; set; }

    [JsonPropertyName("facility")]
    public Facility? Facility { get; set; }

    [Required(ErrorMessage = "La quantità è obbligatoria")]
    [Range(1, 10000, ErrorMessage = "La quantità deve essere tra 1 e 10000")]
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("orderDate")]
    public string? OrderDate { get; set; }

    [JsonPropertyName("deliveryDate")]
    public string? DeliveryDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty; // Pending, InProgress, Completed, Cancelled

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty; // Low, Medium, High, Urgent

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonIgnore]
    public string StatusBadge => Status?.ToLower() switch
    {
        "pending" => "warning",
        "inprogress" => "info",
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
    public string PriorityBadge => Priority?.ToLower() switch
    {
        "low" => "secondary",
        "medium" => "primary",
        "high" => "warning", 
        "urgent" => "danger",
        _ => "secondary"
    };
}
