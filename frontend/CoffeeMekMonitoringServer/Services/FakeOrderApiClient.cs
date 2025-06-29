using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public class FakeOrderApiClient : IOrderService
{
    private readonly List<Order> _orders;
    private readonly List<Customer> _customers;
    private readonly List<Facility> _facilities;
    private int _nextId = 8;

    public FakeOrderApiClient()
    {
        // Dati fake per supporto
        _customers = new List<Customer>
        {
            new() { Id = 1, Name = "Caffè Central Milano", Country = "Italy" },
            new() { Id = 2, Name = "São Paulo Coffee House", Country = "Brasil" },
            new() { Id = 3, Name = "Saigon Premium Coffee", Country = "Vietnam" },
            new() { Id = 4, Name = "Berlin Coffee Roasters", Country = "Germany" }
        };

        _facilities = new List<Facility>
        {
            new() { Id = 1, Name = "Stabilimento Milano", Location = "Italy" },
            new() { Id = 2, Name = "Stabilimento São Paulo", Location = "Brasil" },
            new() { Id = 3, Name = "Stabilimento Ho Chi Minh", Location = "Vietnam" }
        };

        _orders = new List<Order>
        {
            new()
            {
                Id = 1,
                OrderNumber = "ORD-2024-001",
                CustomerId = 1,
                Customer = _customers[0],
                FacilityId = 1,
                Facility = _facilities[0],
                Quantity = 25,
                Status = "InProgress",
                Priority = "High",
                Description = "Macchine espresso professionali - Serie Premium",
                OrderDate = DateTime.UtcNow.AddDays(-15).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                DeliveryDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            },
            new()
            {
                Id = 2,
                OrderNumber = "ORD-2024-002",
                CustomerId = 2,
                Customer = _customers[1],
                FacilityId = 2,
                Facility = _facilities[1],
                Quantity = 50,
                Status = "Pending",
                Priority = "Medium",
                Description = "Macchine caffè automatiche - Serie Standard",
                OrderDate = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                DeliveryDate = DateTime.UtcNow.AddDays(45).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            },
            new()
            {
                Id = 3,
                OrderNumber = "ORD-2024-003",
                CustomerId = 3,
                Customer = _customers[2],
                FacilityId = 3,
                Facility = _facilities[2],
                Quantity = 15,
                Status = "Completed",
                Priority = "Low",
                Description = "Macchine da bar compatte - Serie Compact",
                OrderDate = DateTime.UtcNow.AddDays(-45).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                DeliveryDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            },
            new()
            {
                Id = 4,
                OrderNumber = "ORD-2024-004",
                CustomerId = 4,
                Customer = _customers[3],
                FacilityId = 1,
                Facility = _facilities[0],
                Quantity = 35,
                Status = "InProgress",
                Priority = "Urgent",
                Description = "Macchine espresso industriali - Serie Industrial",
                OrderDate = DateTime.UtcNow.AddDays(-8).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                DeliveryDate = DateTime.UtcNow.AddDays(20).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            },
            new()
            {
                Id = 5,
                OrderNumber = "ORD-2024-005",
                CustomerId = 1,
                Customer = _customers[0],
                FacilityId = 2,
                Facility = _facilities[1],
                Quantity = 20,
                Status = "Pending",
                Priority = "Medium",
                Description = "Macchine cappuccino automatiche - Serie Pro",
                OrderDate = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                DeliveryDate = DateTime.UtcNow.AddDays(60).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            },
            new()
            {
                Id = 6,
                OrderNumber = "ORD-2024-006",
                CustomerId = 2,
                Customer = _customers[1],
                FacilityId = 3,
                Facility = _facilities[2],
                Quantity = 40,
                Status = "Cancelled",
                Priority = "Low",
                Description = "Macchine da ufficio - Serie Office",
                OrderDate = DateTime.UtcNow.AddDays(-20).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                DeliveryDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            },
            new()
            {
                Id = 7,
                OrderNumber = "ORD-2024-007",
                CustomerId = 3,
                Customer = _customers[2],
                FacilityId = 1,
                Facility = _facilities[0],
                Quantity = 60,
                Status = "Pending",
                Priority = "High",
                Description = "Macchine premium per hotel - Serie Luxury",
                OrderDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                DeliveryDate = DateTime.UtcNow.AddDays(90).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            }
        };
    }

    public async Task<ApiResponse<List<Order>>> GetAllOrdersAsync()
    {
        await Task.Delay(600);
        return ApiResponse<List<Order>>.SuccessResult(_orders);
    }

    public async Task<ApiResponse<Order>> GetOrderByIdAsync(int id)
    {
        await Task.Delay(300);
        var order = _orders.FirstOrDefault(o => o.Id == id);
        return order != null
            ? ApiResponse<Order>.SuccessResult(order)
            : ApiResponse<Order>.ErrorResult("Ordine non trovato", 404);
    }

    public async Task<ApiResponse<List<Order>>> GetOrdersByCustomerAsync(int customerId)
    {
        await Task.Delay(400);
        var orders = _orders.Where(o => o.CustomerId == customerId).ToList();
        return ApiResponse<List<Order>>.SuccessResult(orders);
    }

    public async Task<ApiResponse<List<Order>>> GetOrdersByStatusAsync(string status)
    {
        await Task.Delay(400);
        var orders = _orders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        return ApiResponse<List<Order>>.SuccessResult(orders);
    }

    public async Task<ApiResponse<Order>> CreateOrderAsync(Order order)
    {
        await Task.Delay(800);
        
        var newOrder = new Order
        {
            Id = _nextId++,
            OrderNumber = $"ORD-2024-{_nextId:D3}",
            CustomerId = order.CustomerId,
            Customer = _customers.FirstOrDefault(c => c.Id == order.CustomerId),
            FacilityId = order.FacilityId,
            Facility = _facilities.FirstOrDefault(f => f.Id == order.FacilityId),
            Quantity = order.Quantity,
            Status = "Pending",
            Priority = order.Priority,
            Description = order.Description,
            OrderDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            DeliveryDate = order.DeliveryDate
        };

        _orders.Add(newOrder);
        return ApiResponse<Order>.SuccessResult(newOrder);
    }

    public async Task<ApiResponse<Order>> UpdateOrderAsync(int id, Order order)
    {
        await Task.Delay(600);
        
        var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
        if (existingOrder == null)
        {
            return ApiResponse<Order>.ErrorResult("Ordine non trovato", 404);
        }

        existingOrder.CustomerId = order.CustomerId;
        existingOrder.Customer = _customers.FirstOrDefault(c => c.Id == order.CustomerId);
        existingOrder.FacilityId = order.FacilityId;
        existingOrder.Facility = _facilities.FirstOrDefault(f => f.Id == order.FacilityId);
        existingOrder.Quantity = order.Quantity;
        existingOrder.Status = order.Status;
        existingOrder.Priority = order.Priority;
        existingOrder.Description = order.Description;
        existingOrder.DeliveryDate = order.DeliveryDate;

        return ApiResponse<Order>.SuccessResult(existingOrder);
    }

    public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
    {
        await Task.Delay(400);
        
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return ApiResponse<bool>.ErrorResult("Ordine non trovato", 404);
        }

        _orders.Remove(order);
        return ApiResponse<bool>.SuccessResult(true);
    }
}
