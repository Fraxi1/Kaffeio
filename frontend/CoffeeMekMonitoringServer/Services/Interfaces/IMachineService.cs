using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface IMachineService
{
    Task<ApiResponse<List<Machine>>> GetAllMachinesAsync();
    Task<ApiResponse<Machine>> GetMachineByIdAsync(int id);
    Task<ApiResponse<List<Machine>>> GetMachinesByFacilityAsync(int facilityId);
}