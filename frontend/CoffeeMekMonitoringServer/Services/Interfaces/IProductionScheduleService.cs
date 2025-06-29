using CoffeeMekMonitoringServer.Models;

namespace CoffeeMekMonitoringServer.Services.Interfaces;

public interface IProductionScheduleService
{
    Task<ApiResponse<List<ProductionSchedule>>> GetAllSchedulesAsync();
    Task<ApiResponse<ProductionSchedule>> GetScheduleByIdAsync(int id);
    Task<ApiResponse<List<ProductionSchedule>>> GetSchedulesByFacilityAsync(int facilityId);
    Task<ApiResponse<List<ProductionSchedule>>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<ProductionSchedule>> CreateScheduleAsync(ProductionSchedule schedule);
    Task<ApiResponse<ProductionSchedule>> UpdateScheduleAsync(int id, ProductionSchedule schedule);
    Task<ApiResponse<bool>> DeleteScheduleAsync(int id);
}