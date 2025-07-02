using System.Text.Json.Serialization;

namespace CoffeeMekMonitoringServer.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }

    // Proprietà aggiuntiva per compatibilità
    public bool IsSuccess => Success;

    public static ApiResponse<T> SuccessResult(T data, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> ErrorResult(string errorMessage, int statusCode = 500)
    {
        return new ApiResponse<T>
        {
            Success = false,
            ErrorMessage = errorMessage,
            StatusCode = statusCode
        };
    }
    
    /// <summary>
    /// Wrapper per le risposte API reali secondo la struttura CoffeeMek
    /// </summary>
    public class ApiResponseWrapper<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
    
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
    
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}