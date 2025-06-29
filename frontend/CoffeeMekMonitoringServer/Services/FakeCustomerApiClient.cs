using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class FakeCustomerApiClient : ICustomerService
{
    private readonly List<Customer> _customers;
    private int _nextId = 6;

    public FakeCustomerApiClient()
    {
        _customers = new List<Customer>
        {
            new()
            {
                Id = 1,
                Name = "Caffè Central Milano",
                Email = "ordini@caffecentral.it",
                Phone = "+39 02 1234567",
                Address = "Via Milano 123, Milano",
                Country = "Italy",
                CreatedAt = DateTime.UtcNow.AddMonths(-6) 
            },
            new()
            {
                Id = 2,
                Name = "São Paulo Coffee House",
                Email = "compras@spcoffee.com.br",
                Phone = "+55 11 98765432",
                Address = "Avenida Paulista 456, São Paulo",
                Country = "Brasil",
                CreatedAt = DateTime.UtcNow.AddMonths(-4) 
            },
            new()
            {
                Id = 3,
                Name = "Saigon Premium Coffee",
                Email = "info@saigonpremium.vn",
                Phone = "+84 28 87654321",
                Address = "District 1, Ho Chi Minh City",
                Country = "Vietnam",
                CreatedAt = DateTime.UtcNow.AddMonths(-3) 
            },
            new()
            {
                Id = 4,
                Name = "Berlin Coffee Roasters",
                Email = "bestellung@berlincoffee.de",
                Phone = "+49 30 12345678",
                Address = "Unter den Linden 789, Berlin",
                Country = "Germany",
                CreatedAt = DateTime.UtcNow.AddMonths(-2) 
            },
            new()
            {
                Id = 5,
                Name = "Café de Paris",
                Email = "commandes@cafedeparis.fr",
                Phone = "+33 1 23456789",
                Address = "Champs-Élysées 101, Paris",
                Country = "France",
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            }
        };
    }

    public async Task<ApiResponse<List<Customer>>> GetAllCustomersAsync()
    {
        await Task.Delay(500);
        return ApiResponse<List<Customer>>.SuccessResult(_customers);
    }

    public async Task<ApiResponse<Customer>> GetCustomerByIdAsync(int id)
    {
        await Task.Delay(300);
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        return customer != null
            ? ApiResponse<Customer>.SuccessResult(customer)
            : ApiResponse<Customer>.ErrorResult("Cliente non trovato", 404);
    }

    public async Task<ApiResponse<Customer>> CreateCustomerAsync(Customer customer)
    {
        await Task.Delay(700);
        
        if (_customers.Any(c => c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return ApiResponse<Customer>.ErrorResult("Email già esistente", 409);
        }

        var newCustomer = new Customer
        {
            Id = _nextId++,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            Country = customer.Country,
            CreatedAt = DateTime.UtcNow
        };

        _customers.Add(newCustomer);
        return ApiResponse<Customer>.SuccessResult(newCustomer);
    }

    public async Task<ApiResponse<Customer>> UpdateCustomerAsync(int id, Customer customer)
    {
        await Task.Delay(600);
        
        var existingCustomer = _customers.FirstOrDefault(c => c.Id == id);
        if (existingCustomer == null)
        {
            return ApiResponse<Customer>.ErrorResult("Cliente non trovato", 404);
        }

        existingCustomer.Name = customer.Name;
        existingCustomer.Email = customer.Email;
        existingCustomer.Phone = customer.Phone;
        existingCustomer.Address = customer.Address;
        existingCustomer.Country = customer.Country;

        return ApiResponse<Customer>.SuccessResult(existingCustomer);
    }

    public async Task<ApiResponse<bool>> DeleteCustomerAsync(int id)
    {
        await Task.Delay(400);
        
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer == null)
        {
            return ApiResponse<bool>.ErrorResult("Cliente non trovato", 404);
        }

        _customers.Remove(customer);
        return ApiResponse<bool>.SuccessResult(true);
    }
}
