using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface ILotService
{
    Task<ApiResponse<List<Lot>>> GetAllLotsAsync();
    Task<ApiResponse<Lot>> GetLotByIdAsync(int id);
}