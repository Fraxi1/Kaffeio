using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class Lot
{
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("Code")]
    [Required(ErrorMessage = "Il codice lotto è obbligatorio")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("Description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("CreatedAt")]
    public string? CreatedAt { get; set; }

    [JsonPropertyName("Status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("CurrentMachine")]
    public Machine? CurrentMachine { get; set; }

    [JsonIgnore]
    public string StatusBadge => Status switch
    {
        "ok" => "success",
        "warning" => "warning", 
        "error" => "danger",
        _ => "secondary"
    };

    [JsonIgnore]
    public string StatusIcon => Status switch
    {
        "ok" => "fas fa-check-circle",
        "warning" => "fas fa-exclamation-triangle",
        "error" => "fas fa-times-circle",
        _ => "fas fa-question-circle"
    };
}