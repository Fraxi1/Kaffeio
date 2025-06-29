using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public class Fake
{
    
}

 

public class FakeLotApiClient : ILotService
{
    private readonly List<Lot> _lots;

    public FakeLotApiClient()
    {
        _lots = new List<Lot>
        {
            new()
            {
                Id = 1,
                Code = "LOT001",
                Description = "Lotto di produzione caffè Arabica",
                CreatedAt = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                Status = "ok",
                CurrentMachine = new Machine
                {
                    Id = 1,
                    Name = "Macchina Tostatura A1",
                    Type = "tostatura",
                    Status = "operative"
                }
            },
            new()
            {
                Id = 2,
                Code = "LOT002",
                Description = "Lotto di produzione caffè Robusta",
                CreatedAt = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                Status = "warning",
                CurrentMachine = new Machine
                {
                    Id = 2,
                    Name = "Macchina Tostatura B2",
                    Type = "tostatura",
                    Status = "maintenance"
                }
            },
            new()
            {
                Id = 3,
                Code = "LOT003",
                Description = "Lotto miscela espresso",
                CreatedAt = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                Status = "error",
                CurrentMachine = new Machine
                {
                    Id = 3,
                    Name = "Macchina Miscelazione C3",
                    Type = "miscelazione",
                    Status = "offline"
                }
            }
        };
    }

    public async Task<ApiResponse<List<Lot>>> GetAllLotsAsync()
    {
        await Task.Delay(500);
        return ApiResponse<List<Lot>>.SuccessResult(_lots);
    }

    public async Task<ApiResponse<Lot>> GetLotByIdAsync(int id)
    {
        await Task.Delay(300);
        
        var lot = _lots.FirstOrDefault(l => l.Id == id);
        return lot != null
            ? ApiResponse<Lot>.SuccessResult(lot)
            : ApiResponse<Lot>.ErrorResult("Lotto non trovato", 404);
    }
}
 
public class FakeMachineApiClient : IMachineService
{
    private readonly List<Machine> _machines;

    public FakeMachineApiClient()
    {
        _machines = new List<Machine>
        {
            new()
            {
                Id = 1,
                Name = "Macchina Tostatura A1",
                Type = "tostatura",
                FacilityId = 1,
                Status = "operative",
                Facility = new Facility { Id = 1, Name = "Stabilimento Milano", Location = "Italy" }
            },
            new()
            {
                Id = 2,
                Name = "Macchina Tostatura B2",
                Type = "tostatura",
                FacilityId = 1,
                Status = "maintenance",
                Facility = new Facility { Id = 1, Name = "Stabilimento Milano", Location = "Italy" }
            },
            new()
            {
                Id = 3,
                Name = "Macchina Miscelazione C3",
                Type = "miscelazione",
                FacilityId = 2,
                Status = "offline",
                Facility = new Facility { Id = 2, Name = "Stabilimento Roma", Location = "Italy" }
            }
        };
    }

    public async Task<ApiResponse<List<Machine>>> GetAllMachinesAsync()
    {
        await Task.Delay(500);
        return ApiResponse<List<Machine>>.SuccessResult(_machines);
    }

    public async Task<ApiResponse<Machine>> GetMachineByIdAsync(int id)
    {
        await Task.Delay(300);
        
        var machine = _machines.FirstOrDefault(m => m.Id == id);
        return machine != null
            ? ApiResponse<Machine>.SuccessResult(machine)
            : ApiResponse<Machine>.ErrorResult("Macchina non trovata", 404);
    }

    public async Task<ApiResponse<List<Machine>>> GetMachinesByFacilityAsync(int facilityId)
    {
        await Task.Delay(400);
        
        var machines = _machines.Where(m => m.FacilityId == facilityId).ToList();
        return ApiResponse<List<Machine>>.SuccessResult(machines);
    }
}

 

public class FakeFacilityApiClient : IFacilityService
{
    private readonly List<Facility> _facilities;

    public FakeFacilityApiClient()
    {
        _facilities = new List<Facility>
        {
            new()
            {
                Id = 1,
                Name = "Stabilimento Milano",
                Location = "Italy",
                TimeZone = "+1"
            },
            new()
            {
                Id = 2,
                Name = "Stabilimento Roma",
                Location = "Italy",
                TimeZone = "+1"
            },
            new()
            {
                Id = 3,
                Name = "Facility Berlin",
                Location = "Germany",
                TimeZone = "+1"
            }
        };
    }

    public async Task<ApiResponse<List<Facility>>> GetAllFacilitiesAsync()
    {
        await Task.Delay(500);
        return ApiResponse<List<Facility>>.SuccessResult(_facilities);
    }

    public async Task<ApiResponse<Facility>> GetFacilityByIdAsync(int id)
    {
        await Task.Delay(300);
        
        var facility = _facilities.FirstOrDefault(f => f.Id == id);
        return facility != null
            ? ApiResponse<Facility>.SuccessResult(facility)
            : ApiResponse<Facility>.ErrorResult("Facility non trovata", 404);
    }
}
