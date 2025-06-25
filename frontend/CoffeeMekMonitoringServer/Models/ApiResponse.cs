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
}