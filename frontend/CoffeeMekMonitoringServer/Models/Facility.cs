using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class Facility
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Il nome facility è obbligatorio")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    [Required(ErrorMessage = "La location è obbligatoria")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; } = string.Empty;

    [JsonIgnore]
    public string LocationIcon => Location?.ToLower() switch
    {
        "italy" => "fas fa-flag",
        "germany" => "fas fa-flag",
        "france" => "fas fa-flag",
        _ => "fas fa-map-marker-alt"
    };
}