using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<List<Order>>> GetAllOrdersAsync();
    Task<ApiResponse<Order>> GetOrderByIdAsync(int id);
    Task<ApiResponse<List<Order>>> GetOrdersByCustomerAsync(int customerId);
    Task<ApiResponse<List<Order>>> GetOrdersByStatusAsync(string status);
    Task<ApiResponse<Order>> CreateOrderAsync(Order order);
    Task<ApiResponse<Order>> UpdateOrderAsync(int id, Order order);
    Task<ApiResponse<bool>> DeleteOrderAsync(int id);
}