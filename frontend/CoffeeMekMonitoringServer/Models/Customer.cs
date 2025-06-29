using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class Customer
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Il nome cliente è obbligatorio")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email è obbligatoria")]
    [EmailAddress(ErrorMessage = "Formato email non valido")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public string? CreatedAt { get; set; }

    [JsonIgnore]
    public string CountryFlag => Country?.ToLower() switch
    {
        "italy" => "🇮🇹",
        "brasil" => "🇧🇷",
        "vietnam" => "🇻🇳",
        "germany" => "🇩🇪",
        "france" => "🇫🇷",
        _ => "🌍"
    };
}