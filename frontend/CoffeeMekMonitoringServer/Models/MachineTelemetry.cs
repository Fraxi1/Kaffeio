using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class MachineTelemetry
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("machineId")]
    public int MachineId { get; set; }

    [JsonPropertyName("machine")]
    public Machine? Machine { get; set; }

    [JsonPropertyName("lotId")]
    public int? LotId { get; set; }

    [JsonPropertyName("lot")]
    public Lot? Lot { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("timeZone")]
    public string TimeZone { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; } = new();

    // Proprietà specifiche per tipo macchina
    [JsonIgnore]
    public string MachineType => Machine?.Type ?? "";

    [JsonIgnore]
    public bool HasAlerts => Data.ContainsKey("alerts") && Data["alerts"]?.ToString() != "0";

    [JsonIgnore]
    public string AlertLevel => HasAlerts ? "danger" : "success";
}