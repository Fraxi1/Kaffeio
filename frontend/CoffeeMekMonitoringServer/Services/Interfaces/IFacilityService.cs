using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface IFacilityService
{
    Task<ApiResponse<List<Facility>>> GetAllFacilitiesAsync();
    Task<ApiResponse<Facility>> GetFacilityByIdAsync(int id);
}