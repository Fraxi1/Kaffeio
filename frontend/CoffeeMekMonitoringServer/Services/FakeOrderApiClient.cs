using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class FakeOrderApiClient : IOrderService
{
    private readonly List<Order> _orders;
    private readonly List<OrderCustomer> _customers;
    private readonly List<Facility> _facilities;
    private readonly ILogger<FakeOrderApiClient> _logger;
    private int _nextId = 15;

    public FakeOrderApiClient(ILogger<FakeOrderApiClient> logger)
    {
        _logger = logger;
        
        // Clienti globali CoffeeMek secondo PDF
        _customers = new List<OrderCustomer>
        {
            new() { Id = 1, Name = "Caffè Central Milano", Email = "ordini@caffecentral.it", Country = "Italy", CreatedAt = DateTime.UtcNow.AddMonths(-12) },
            new() { Id = 2, Name = "São Paulo Coffee House", Email = "pedidos@spcoffee.com.br", Country = "Brasil", CreatedAt = DateTime.UtcNow.AddMonths(-8) },
            new() { Id = 3, Name = "Saigon Premium Coffee", Email = "orders@saigoncoffee.vn", Country = "Vietnam", CreatedAt = DateTime.UtcNow.AddMonths(-6) },
            new() { Id = 4, Name = "Berlin Coffee Roasters", Email = "bestellungen@berlincoffee.de", Country = "Germany", CreatedAt = DateTime.UtcNow.AddMonths(-10) },
            new() { Id = 5, Name = "Paris Café Excellence", Email = "commandes@cafeparis.fr", Country = "France", CreatedAt = DateTime.UtcNow.AddMonths(-4) },
            new() { Id = 6, Name = "Barcelona Coffee Culture", Email = "pedidos@bcnculture.es", Country = "Spain", CreatedAt = DateTime.UtcNow.AddMonths(-7) },
            new() { Id = 7, Name = "New York Premium Roasters", Email = "orders@nyroasters.com", Country = "USA", CreatedAt = DateTime.UtcNow.AddMonths(-15) },
            new() { Id = 8, Name = "Hotel Luxury Chain International", Email = "procurement@luxurychain.com", Country = "Italy", CreatedAt = DateTime.UtcNow.AddMonths(-24) }
        };

        // Sedi produttive CoffeeMek secondo PDF
        _facilities = new List<Facility>
        {
            new() { Id = 1, Name = "Stabilimento Milano", Location = "Italy", TimeZone = "+1"  },
            new() { Id = 2, Name = "Fábrica São Paulo", Location = "Brasil", TimeZone = "-3"  },
            new() { Id = 3, Name = "CoffeeMek Ho Chi Minh Factory", Location = "Vietnam"  }
        };

        // Ordini fake realistici con struttura API reale
        _orders = CreateFakeOrders();
    }

    private List<Order> CreateFakeOrders()
    {
        var orders = new List<Order>();
        var random = new Random();
        var statuses = new[] { "Pending", "InProgress", "Completed", "Cancelled" };
        var notes = new[]
        {
            "Macchine espresso professionali - Serie Premium per hotel luxury",
            "Macchine caffè automatiche - Serie Standard per catene ristoranti",
            "Macchine da bar compatte - Serie Compact per caffetterie",
            "Macchine espresso industriali - Serie Industrial per aeroporti",
            "Macchine cappuccino automatiche - Serie Pro per uffici",
            "Macchine da ufficio - Serie Office per corporate",
            "Macchine premium per hotel - Serie Luxury per hospitality",
            "Macchine semi-automatiche - Serie Classic per bar tradizionali"
        };

        for (int i = 1; i <= 14; i++)
        {
            var customer = _customers[random.Next(_customers.Count)];
            var facility = EstimateFacilityFromCustomer(customer);
            var status = statuses[random.Next(statuses.Length)];
            var orderDate = DateTime.UtcNow.AddDays(-random.Next(1, 60));
            var deliveryDate = status == "Completed" ? orderDate.AddDays(random.Next(20, 40)) : orderDate.AddDays(random.Next(30, 90));

            var order = new Order
            {
                Id = i,
                OrderNumber = $"CFM-{DateTime.Now.Year}-{i:D4}",
                CustomerId = customer.Id,
                Customer = customer,
                Quantity = random.Next(10, 100),
                OrderDate = orderDate,
                DeliveryDate = status == "Cancelled" ? null : deliveryDate,
                Status = status,
                Notes = notes[random.Next(notes.Length)],
                Lots = GenerateOrderLots(i, random)
            };

            // Aggiungi proprietà di compatibilità
            order.FacilityId = facility.Id;
            order.Facility = facility;
            order.Priority = GetRandomPriority(random);
            order.Description = order.Notes;

            orders.Add(order);
        }

        return orders.OrderByDescending(o => o.OrderDate).ToList();
    }

    private List<OrderLot> GenerateOrderLots(int orderId, Random random)
    {
        var lots = new List<OrderLot>();
        var lotCount = random.Next(1, 4); // 1-3 lotti per ordine
        var phases = new[] { "Fresa", "Tornio", "Assemblaggio", "Test" };
        var lotStatuses = new[] { "pending", "in_progress", "completed", "error" };

        for (int i = 1; i <= lotCount; i++)
        {
            var lot = new OrderLot
            {
                Id = orderId * 10 + i,
                Code = $"LOT-CFM-{orderId:D3}-{i:D2}",
                Description = $"Lotto {i} - Macchine da caffè CoffeeMek",
                Status = lotStatuses[random.Next(lotStatuses.Length)],
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                CurrentPhase = phases[random.Next(phases.Length)],
                Progress = random.Next(0, 101)
            };
            lots.Add(lot);
        }

        return lots;
    }

    private Facility EstimateFacilityFromCustomer(OrderCustomer customer)
    {
        return customer.Country?.ToLower() switch
        {
            "italy" => _facilities[0], // Milano
            "brasil" => _facilities[1], // São Paulo
            "vietnam" => _facilities[2], // Ho Chi Minh
            _ => _facilities[0] // Default Milano
        };
    }

    private string GetRandomPriority(Random random)
    {
        var priorities = new[] { "Low", "Medium", "High", "Urgent" };
        return priorities[random.Next(priorities.Length)];
    }

    public async Task<ApiResponse<List<Order>>> GetAllOrdersAsync()
    {
        try
        {
            _logger.LogDebug("Fetching all fake CoffeeMek orders");
            await Task.Delay(Random.Shared.Next(300, 800));
            
            _logger.LogInformation("Successfully fetched {Count} fake CoffeeMek orders", _orders.Count);
            return ApiResponse<List<Order>>.SuccessResult(_orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fake CoffeeMek orders");
            return ApiResponse<List<Order>>.ErrorResult($"Errore simulazione: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Order>> GetOrderByIdAsync(int id)
    {
        try
        {
            _logger.LogDebug("Fetching fake CoffeeMek order {OrderId}", id);
            await Task.Delay(Random.Shared.Next(200, 500));
            
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                _logger.LogInformation("Successfully fetched fake order {OrderId}: {OrderNumber}", id, order.OrderNumber);
                return ApiResponse<Order>.SuccessResult(order);
            }
            else
            {
                _logger.LogWarning("Fake order {OrderId} not found", id);
                return ApiResponse<Order>.ErrorResult("Commessa CoffeeMek non trovata", 404);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fake order {OrderId}", id);
            return ApiResponse<Order>.ErrorResult($"Errore simulazione: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Order>>> GetOrdersByCustomerAsync(int customerId)
    {
        try
        {
            _logger.LogDebug("Fetching fake orders for customer {CustomerId}", customerId);
            await Task.Delay(Random.Shared.Next(300, 600));
            
            var orders = _orders.Where(o => o.CustomerId == customerId).ToList();
            _logger.LogInformation("Found {Count} fake orders for customer {CustomerId}", orders.Count, customerId);
            return ApiResponse<List<Order>>.SuccessResult(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fake orders for customer {CustomerId}", customerId);
            return ApiResponse<List<Order>>.ErrorResult($"Errore simulazione: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Order>>> GetOrdersByStatusAsync(string status)
    {
        try
        {
            _logger.LogDebug("Fetching fake CoffeeMek orders by status: {Status}", status);
            await Task.Delay(Random.Shared.Next(300, 600));
        
            var orders = _orders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
        
            _logger.LogInformation("Found {Count} fake CoffeeMek orders with status: {Status}", orders.Count, status);
            return ApiResponse<List<Order>>.SuccessResult(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching fake orders by status {Status}", status);
            return ApiResponse<List<Order>>.ErrorResult($"Errore simulazione filtro stato: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Order>> CreateOrderAsync(Order order)
    {
        try
        {
            _logger.LogDebug("Creating fake CoffeeMek order for customer {CustomerId}", order.CustomerId);
            await Task.Delay(Random.Shared.Next(800, 1500));
            
            var customer = _customers.FirstOrDefault(c => c.Id == order.CustomerId);
            var facility = _facilities.FirstOrDefault(f => f.Id == order.FacilityId) ?? EstimateFacilityFromCustomer(customer);
            
            var newOrder = new Order
            {
                Id = _nextId++,
                OrderNumber = $"CFM-{DateTime.Now.Year}-{_nextId:D4}",
                CustomerId = order.CustomerId,
                Customer = customer,
                Quantity = order.Quantity,
                OrderDate = DateTime.UtcNow,
                DeliveryDate = order.DeliveryDate,
                Status = "Pending",
                Notes = order.Notes ?? order.Description,
                Lots = new List<OrderLot>(),
                
                // Proprietà di compatibilità
                FacilityId = facility?.Id ?? 1,
                Facility = facility,
                Priority = order.Priority ?? "Medium",
                Description = order.Description ?? order.Notes ?? "Commessa macchine da caffè CoffeeMek"
            };

            _orders.Insert(0, newOrder); // Aggiungi all'inizio per ordine cronologico
            
            _logger.LogInformation("Successfully created fake CoffeeMek order: {OrderNumber} with ID: {OrderId}", 
                newOrder.OrderNumber, newOrder.Id);
            return ApiResponse<Order>.SuccessResult(newOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating fake order for customer {CustomerId}", order.CustomerId);
            return ApiResponse<Order>.ErrorResult($"Errore nella simulazione di creazione: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Order>> UpdateOrderAsync(int id, Order order)
    {
        try
        {
            _logger.LogDebug("Updating fake CoffeeMek order {OrderId}", id);
            await Task.Delay(Random.Shared.Next(600, 1200));
            
            var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
            if (existingOrder == null)
            {
                _logger.LogWarning("Fake order {OrderId} not found for update", id);
                return ApiResponse<Order>.ErrorResult("Commessa CoffeeMek non trovata", 404);
            }

            var customer = _customers.FirstOrDefault(c => c.Id == order.CustomerId);
            var facility = _facilities.FirstOrDefault(f => f.Id == order.FacilityId);

            // Aggiorna i campi
            existingOrder.CustomerId = order.CustomerId;
            existingOrder.Customer = customer;
            existingOrder.FacilityId = order.FacilityId;
            existingOrder.Facility = facility;
            existingOrder.Quantity = order.Quantity;
            existingOrder.Status = order.Status;
            existingOrder.Priority = order.Priority;
            existingOrder.Description = order.Description;
            existingOrder.Notes = order.Notes ?? order.Description;
            existingOrder.DeliveryDate = order.DeliveryDate;

            _logger.LogInformation("Successfully updated fake CoffeeMek order {OrderId}", id);
            return ApiResponse<Order>.SuccessResult(existingOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating fake order {OrderId}", id);
            return ApiResponse<Order>.ErrorResult($"Errore nella simulazione di aggiornamento: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
    {
        try
        {
            _logger.LogDebug("Deleting fake CoffeeMek order {OrderId}", id);
            await Task.Delay(Random.Shared.Next(400, 800));
            
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                _logger.LogWarning("Fake order {OrderId} not found for deletion", id);
                return ApiResponse<bool>.ErrorResult("Commessa CoffeeMek non trovata", 404);
            }

            // Simula controllo dipendenze
            if (order.Status == "InProgress" && order.Lots.Any(l => l.Status == "in_progress"))
            {
                _logger.LogWarning("Cannot delete order {OrderId} - has active lots", id);
                return ApiResponse<bool>.ErrorResult("Impossibile eliminare: commessa ha lotti in produzione", 409);
            }

            _orders.Remove(order);
            _logger.LogInformation("Successfully deleted fake CoffeeMek order {OrderId}", id);
            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting fake order {OrderId}", id);
            return ApiResponse<bool>.ErrorResult($"Errore nella simulazione di cancellazione: {ex.Message}");
        }
    }
}
