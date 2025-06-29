using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface ICustomerService
{
    Task<ApiResponse<List<Customer>>> GetAllCustomersAsync();
    Task<ApiResponse<Customer>> GetCustomerByIdAsync(int id);
    Task<ApiResponse<Customer>> CreateCustomerAsync(Customer customer);
    Task<ApiResponse<Customer>> UpdateCustomerAsync(int id, Customer customer);
    Task<ApiResponse<bool>> DeleteCustomerAsync(int id);
}