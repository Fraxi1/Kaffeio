using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Il nome è obbligatorio")]
    [StringLength(50, ErrorMessage = "Il nome non può superare 50 caratteri")]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il cognome è obbligatorio")]
    [StringLength(50, ErrorMessage = "Il cognome non può superare 50 caratteri")]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email è obbligatoria")]
    [EmailAddress(ErrorMessage = "Formato email non valido")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
}