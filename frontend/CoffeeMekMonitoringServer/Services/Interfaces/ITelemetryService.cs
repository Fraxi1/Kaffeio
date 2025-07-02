using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface ITelemetryService
{
    Task<ApiResponse<List<MachineTelemetry>>> GetTelemetryByMachineAsync(int machineId, int hours = 24);
    Task<ApiResponse<List<MachineTelemetry>>> GetTelemetryByLotAsync(int lotId);
    Task<ApiResponse<List<MachineTelemetry>>> GetRecentTelemetryAsync(int minutes = 30);
    Task<ApiResponse<Dictionary<string, object>>> GetMachineDashboardDataAsync(int machineId);
    Task<ApiResponse<Dictionary<string, object>>> GetFacilityDashboardDataAsync(int facilityId);
}