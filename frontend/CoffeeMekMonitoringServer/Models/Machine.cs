using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class Machine
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Il nome macchina è obbligatorio")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    [Required(ErrorMessage = "Il tipo macchina è obbligatorio")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("facilityId")]
    public int FacilityId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("facility")]
    public Facility? Facility { get; set; }

    [JsonIgnore]
    public string StatusBadge => Status switch
    {
        "operative" => "success",
        "maintenance" => "warning",
        "offline" => "danger",
        _ => "secondary"
    };

    [JsonIgnore]
    public string StatusIcon => Status switch
    {
        "operative" => "fas fa-play-circle",
        "maintenance" => "fas fa-tools",
        "offline" => "fas fa-stop-circle",
        _ => "fas fa-question-circle"
    };

    [JsonIgnore]
    public string TypeIcon => Type?.ToLower() switch
    {
        "fresa" => "fas fa-cog",
        "tornio" => "fas fa-circle-notch",
        "pressa" => "fas fa-compress-arrows-alt",
        _ => "fas fa-industry"
    };
}