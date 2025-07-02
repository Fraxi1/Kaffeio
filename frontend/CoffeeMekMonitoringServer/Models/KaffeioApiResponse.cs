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

 

// Response per Lots
public class LotsListResponse : KaffeioApiResponse<List<Lot>> { }
public class LotResponse : KaffeioApiResponse<Lot> { }

// Response per Machines
public class MachinesListResponse : KaffeioApiResponse<List<Machine>> { }
public class MachineResponse : KaffeioApiResponse<Machine> { }

// Response per Facilities
public class FacilitiesListResponse : KaffeioApiResponse<List<Facility>> { }
public class FacilityResponse : KaffeioApiResponse<Facility> { }

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
