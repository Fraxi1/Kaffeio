using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class ProductionSchedule
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("lotId")]
    public int LotId { get; set; }

    [JsonPropertyName("lot")]
    public Lot? Lot { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("order")]
    public Order? Order { get; set; }

    [JsonPropertyName("facilityId")]
    public int FacilityId { get; set; }

    [JsonPropertyName("facility")]
    public Facility? Facility { get; set; }

    [JsonPropertyName("scheduledDate")]
    public string? ScheduledDate { get; set; }

    [JsonPropertyName("startDate")]
    public string? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }

    [JsonPropertyName("currentPhase")]
    public string CurrentPhase { get; set; } = string.Empty; // Fresa, Tornio, Assemblaggio, Test

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty; // Scheduled, InProgress, Completed, Delayed

    [JsonPropertyName("progress")]
    public int Progress { get; set; } = 0; // 0-100%

    [JsonIgnore]
    public string PhaseIcon => CurrentPhase?.ToLower() switch
    {
        "fresa" => "fas fa-cog",
        "tornio" => "fas fa-circle-notch",
        "assemblaggio" => "fas fa-puzzle-piece",
        "test" => "fas fa-vial",
        _ => "fas fa-question"
    };

    [JsonIgnore]
    public string StatusBadge => Status?.ToLower() switch
    {
        "scheduled" => "secondary",
        "inprogress" => "primary",
        "completed" => "success",
        "delayed" => "danger",
        _ => "secondary"
    };
}