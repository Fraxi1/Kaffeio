namespace CoffeeMekMonitoringServer.Models;

 
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public User User { get; set; } = new();
}
