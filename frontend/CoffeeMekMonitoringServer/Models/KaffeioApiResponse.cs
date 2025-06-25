namespace CoffeeMekMonitoringServer.Models;

using System.Text.Json.Serialization;

 
// Classe generica per tutte le risposte dell'API Kaffeio
public class KaffeioApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }
}

// Specializzazioni per diversi tipi di response
public class UsersListResponse : KaffeioApiResponse<List<User>>
{
}

public class UserResponse : KaffeioApiResponse<User>
{
}

public class UserCreateResponse : KaffeioApiResponse<User>
{
}

public class UserDeleteResponse : KaffeioApiResponse<object>
{
}
