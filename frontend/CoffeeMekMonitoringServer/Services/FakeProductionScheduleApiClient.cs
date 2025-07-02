using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class FakeProductionScheduleApiClient : IProductionScheduleService
{
    private readonly List<ProductionSchedule> _schedules;
    private readonly List<Order> _orders;
    private readonly List<Lot> _lots;
    private readonly List<Facility> _facilities;
    private int _nextId = 8;

    public FakeProductionScheduleApiClient()
    {
        // Dati di supporto (fake)
        _facilities = new List<Facility>
        {
            new() { Id = 1, Name = "Stabilimento Milano", Location = "Italy", TimeZone = "+1" },
            new() { Id = 2, Name = "Stabilimento São Paulo", Location = "Brasil", TimeZone = "-3" },
            new() { Id = 3, Name = "Stabilimento Ho Chi Minh", Location = "Vietnam", TimeZone = "+7" }
        };

        _orders = new List<Order>
        {
            new() { Id = 1, OrderNumber = "ORD-2024-001", CustomerId = 1 },
            new() { Id = 2, OrderNumber = "ORD-2024-002", CustomerId = 2 },
            new() { Id = 3, OrderNumber = "ORD-2024-003", CustomerId = 3 }
        };

        _lots = new List<Lot>
        {
            new() { Id = 1, Code = "LOT-001", Description = "Lotto Espresso Premium", Status = "ok" },
            new() { Id = 2, Code = "LOT-002", Description = "Lotto Cappuccino Deluxe", Status = "ok" },
            new() { Id = 3, Code = "LOT-003", Description = "Lotto Industrial Pro", Status = "warning" }
        };

        _schedules = new List<ProductionSchedule>
        {
            new()
            {
                Id = 1,
                LotId = 1,
                Lot = _lots[0],
                OrderId = 1,
                Order = _orders[0],
                FacilityId = 1,
                Facility = _facilities[0],
                ScheduledDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                StartDate = DateTime.UtcNow.AddDays(-4).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                EndDate = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                CurrentPhase = "Assemblaggio",
                Status = "InProgress",
                Progress = 75
            },
            new()
            {
                Id = 2,
                LotId = 2,
                Lot = _lots[1],
                OrderId = 2,
                Order = _orders[1],
                FacilityId = 2,
                Facility = _facilities[1],
                ScheduledDate = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                StartDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                EndDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                CurrentPhase = "Tornio",
                Status = "InProgress",
                Progress = 40
            },
            new()
            {
                Id = 3,
                LotId = 3,
                Lot = _lots[2],
                OrderId = 3,
                Order = _orders[2],
                FacilityId = 3,
                Facility = _facilities[2],
                ScheduledDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                StartDate = null,
                EndDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                CurrentPhase = "Fresa",
                Status = "Scheduled",
                Progress = 0
            },
            new()
            {
                Id = 4,
                LotId = 1,
                Lot = _lots[0],
                OrderId = 1,
                Order = _orders[0],
                FacilityId = 1,
                Facility = _facilities[0],
                ScheduledDate = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                StartDate = DateTime.UtcNow.AddDays(-9).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                EndDate = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                CurrentPhase = "Test",
                Status = "Completed",
                Progress = 100
            },
            new()
            {
                Id = 5,
                LotId = 2,
                Lot = _lots[1],
                OrderId = 2,
                Order = _orders[1],
                FacilityId = 2,
                Facility = _facilities[1],
                ScheduledDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                StartDate = null,
                EndDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                CurrentPhase = "Fresa",
                Status = "Delayed",
                Progress = 0
            },
            new()
            {
                Id = 6,
                LotId = 3,
                Lot = _lots[2],
                OrderId = 3,
                Order = _orders[2],
                FacilityId = 1,
                Facility = _facilities[0],
                ScheduledDate = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                StartDate = DateTime.UtcNow.AddDays(-6).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                EndDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                CurrentPhase = "Test",
                Status = "InProgress",
                Progress = 95
            },
            new()
            {
                Id = 7,
                LotId = 1,
                Lot = _lots[0],
                OrderId = 1,
                Order = _orders[0],
                FacilityId = 3,
                Facility = _facilities[2],
                ScheduledDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                StartDate = null,
                EndDate = DateTime.UtcNow.AddDays(20).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                CurrentPhase = "Fresa",
                Status = "Scheduled",
                Progress = 0
            }
        };
    }

    public async Task<ApiResponse<List<ProductionSchedule>>> GetAllSchedulesAsync()
    {
        await Task.Delay(600);
        return ApiResponse<List<ProductionSchedule>>.SuccessResult(_schedules);
    }

    public async Task<ApiResponse<ProductionSchedule>> GetScheduleByIdAsync(int id)
    {
        await Task.Delay(300);
        var schedule = _schedules.FirstOrDefault(s => s.Id == id);
        return schedule != null
            ? ApiResponse<ProductionSchedule>.SuccessResult(schedule)
            : ApiResponse<ProductionSchedule>.ErrorResult("Schedulazione non trovata", 404);
    }

    public async Task<ApiResponse<List<ProductionSchedule>>> GetSchedulesByFacilityAsync(int facilityId)
    {
        await Task.Delay(400);
        var schedules = _schedules.Where(s => s.FacilityId == facilityId).ToList();
        return ApiResponse<List<ProductionSchedule>>.SuccessResult(schedules);
    }

    public async Task<ApiResponse<List<ProductionSchedule>>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        await Task.Delay(500);
        var schedules = _schedules.Where(s =>
        {
            if (DateTime.TryParse(s.ScheduledDate, out var scheduledDate))
            {
                return scheduledDate >= startDate && scheduledDate <= endDate;
            }
            return false;
        }).ToList();
        return ApiResponse<List<ProductionSchedule>>.SuccessResult(schedules);
    }

    public async Task<ApiResponse<ProductionSchedule>> CreateScheduleAsync(ProductionSchedule schedule)
    {
        await Task.Delay(800);
        
        var newSchedule = new ProductionSchedule
        {
            Id = _nextId++,
            LotId = schedule.LotId,
            Lot = _lots.FirstOrDefault(l => l.Id == schedule.LotId),
            OrderId = schedule.OrderId,
            Order = _orders.FirstOrDefault(o => o.Id == schedule.OrderId),
            FacilityId = schedule.FacilityId,
            Facility = _facilities.FirstOrDefault(f => f.Id == schedule.FacilityId),
            ScheduledDate = schedule.ScheduledDate,
            StartDate = schedule.StartDate,
            EndDate = schedule.EndDate,
            CurrentPhase = schedule.CurrentPhase,
            Status = "Scheduled",
            Progress = 0
        };

        _schedules.Add(newSchedule);
        return ApiResponse<ProductionSchedule>.SuccessResult(newSchedule);
    }

    public async Task<ApiResponse<ProductionSchedule>> UpdateScheduleAsync(int id, ProductionSchedule schedule)
    {
        await Task.Delay(600);
        
        var existingSchedule = _schedules.FirstOrDefault(s => s.Id == id);
        if (existingSchedule == null)
        {
            return ApiResponse<ProductionSchedule>.ErrorResult("Schedulazione non trovata", 404);
        }

        existingSchedule.LotId = schedule.LotId;
        existingSchedule.OrderId = schedule.OrderId;
        existingSchedule.FacilityId = schedule.FacilityId;
        existingSchedule.ScheduledDate = schedule.ScheduledDate;
        existingSchedule.StartDate = schedule.StartDate;
        existingSchedule.EndDate = schedule.EndDate;
        existingSchedule.CurrentPhase = schedule.CurrentPhase;
        existingSchedule.Status = schedule.Status;
        existingSchedule.Progress = schedule.Progress;

        return ApiResponse<ProductionSchedule>.SuccessResult(existingSchedule);
    }

    public async Task<ApiResponse<bool>> DeleteScheduleAsync(int id)
    {
        await Task.Delay(400);
        
        var schedule = _schedules.FirstOrDefault(s => s.Id == id);
        if (schedule == null)
        {
            return ApiResponse<bool>.ErrorResult("Schedulazione non trovata", 404);
        }

        _schedules.Remove(schedule);
        return ApiResponse<bool>.SuccessResult(true);
    }
}
