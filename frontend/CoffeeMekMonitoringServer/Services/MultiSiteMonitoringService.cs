using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services;

public interface IMultiSiteMonitoringService
{
    Task<Dictionary<string, object>> GetGlobalStatusAsync();
    Task<Dictionary<string, object>> GetSiteStatusAsync(int facilityId);
    Task<List<object>> GetCrossSiteWorkflowAsync();
}

public class MultiSiteMonitoringService : IMultiSiteMonitoringService
{
    private readonly IFacilityService _facilityService;
    private readonly IMachineService _machineService;
    private readonly ITelemetryService _telemetryService;
    private readonly ILogger<MultiSiteMonitoringService> _logger;

    public MultiSiteMonitoringService(
        IFacilityService facilityService,
        IMachineService machineService,
        ITelemetryService telemetryService,
        ILogger<MultiSiteMonitoringService> logger)
    {
        _facilityService = facilityService;
        _machineService = machineService;
        _telemetryService = telemetryService;
        _logger = logger;
    }

    public async Task<Dictionary<string, object>> GetGlobalStatusAsync()
    {
        try
        {
            var facilitiesResult = await _facilityService.GetAllFacilitiesAsync();
            var machinesResult = await _machineService.GetAllMachinesAsync();

            if (!facilitiesResult.IsSuccess || !machinesResult.IsSuccess)
            {
                return new Dictionary<string, object> { ["error"] = "Errore nel caricamento dati" };
            }

            var facilities = facilitiesResult.Data!;
            var machines = machinesResult.Data!;

            return new Dictionary<string, object>
            {
                ["totalFacilities"] = facilities.Count,
                ["totalMachines"] = machines.Count,
                ["operativeMachines"] = machines.Count(m => m.Status == "operative"),
                ["sitesStatus"] = facilities.Select(f => new
                {
                    f.Id,
                    f.Name,
                    f.Location,
                    f.TimeZone,
                    LocalTime = GetLocalTime(f.TimeZone),
                    Machines = machines.Where(m => m.FacilityId == f.Id).Count(),
                    Operative = machines.Where(m => m.FacilityId == f.Id && m.Status == "operative").Count()
                }).ToList(),
                ["lastUpdate"] = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore nel calcolo dello status globale");
            return new Dictionary<string, object> { ["error"] = ex.Message };
        }
    }

    public async Task<Dictionary<string, object>> GetSiteStatusAsync(int facilityId)
    {
        try
        {
            var facilityResult = await _facilityService.GetFacilityByIdAsync(facilityId);
            var machinesResult = await _machineService.GetMachinesByFacilityAsync(facilityId);

            if (!facilityResult.IsSuccess || !machinesResult.IsSuccess)
            {
                return new Dictionary<string, object> { ["error"] = "Sede non trovata" };
            }

            var facility = facilityResult.Data!;
            var machines = machinesResult.Data!;

            // Statistiche per workflow (secondo PDF CoffeeMek)
            var workflowStats = new[]
            {
                new { Phase = "fresa", Name = "Fresa CNC", Icon = "fas fa-cog" },
                new { Phase = "tornio", Name = "Tornio", Icon = "fas fa-circle-notch" },
                new { Phase = "assemblaggio", Name = "Assemblaggio", Icon = "fas fa-puzzle-piece" },
                new { Phase = "test", Name = "Test", Icon = "fas fa-vial" }
            }.Select(phase => new
            {
                phase.Phase,
                phase.Name,
                phase.Icon,
                Total = machines.Count(m => m.Type == phase.Phase),
                Operative = machines.Count(m => m.Type == phase.Phase && m.Status == "operative")
            }).ToList();

            return new Dictionary<string, object>
            {
                ["facility"] = facility,
                ["localTime"] = GetLocalTime(facility.TimeZone),
                ["totalMachines"] = machines.Count,
                ["operativeMachines"] = machines.Count(m => m.Status == "operative"),
                ["workflowStats"] = workflowStats,
                ["efficiency"] = machines.Count > 0 ? 
                    Math.Round((double)machines.Count(m => m.Status == "operative") / machines.Count * 100, 1) : 0,
                ["lastUpdate"] = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore nel calcolo dello status della sede {FacilityId}", facilityId);
            return new Dictionary<string, object> { ["error"] = ex.Message };
        }
    }

    public async Task<List<object>> GetCrossSiteWorkflowAsync()
    {
        try
        {
            var facilitiesResult = await _facilityService.GetAllFacilitiesAsync();
            var machinesResult = await _machineService.GetAllMachinesAsync();

            if (!facilitiesResult.IsSuccess || !machinesResult.IsSuccess)
            {
                return new List<object>();
            }

            var facilities = facilitiesResult.Data!;
            var machines = machinesResult.Data!;

            // Workflow cross-site secondo il PDF: Fresa CNC → Tornio → Assemblaggio → Test
            return facilities.Select(facility =>
            {
                var facilityMachines = machines.Where(m => m.FacilityId == facility.Id).ToList();
                
                return new
                {
                    facility.Id,
                    facility.Name,
                    facility.Location,
                    Flag = GetFacilityFlag(facility.Location),
                    TimeZone = facility.TimeZone,
                    LocalTime = GetLocalTime(facility.TimeZone),
                    Workflow = new[]
                    {
                        new { Phase = "Fresa CNC", Type = "fresa", Machines = facilityMachines.Count(m => m.Type == "fresa"), Operative = facilityMachines.Count(m => m.Type == "fresa" && m.Status == "operative") },
                        new { Phase = "Tornio", Type = "tornio", Machines = facilityMachines.Count(m => m.Type == "tornio"), Operative = facilityMachines.Count(m => m.Type == "tornio" && m.Status == "operative") },
                        new { Phase = "Assemblaggio", Type = "assemblaggio", Machines = facilityMachines.Count(m => m.Type == "assemblaggio"), Operative = facilityMachines.Count(m => m.Type == "assemblaggio" && m.Status == "operative") },
                        new { Phase = "Test", Type = "test", Machines = facilityMachines.Count(m => m.Type == "test"), Operative = facilityMachines.Count(m => m.Type == "test" && m.Status == "operative") }
                    }
                };
            }).ToList<object>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore nel calcolo del workflow cross-site");
            return new List<object>();
        }
    }

    private string GetLocalTime(string timeZone)
    {
        try
        {
            var offset = int.Parse(timeZone.Replace("+", ""));
            return DateTime.UtcNow.AddHours(offset).ToString("HH:mm:ss");
        }
        catch
        {
            return DateTime.UtcNow.ToString("HH:mm:ss");
        }
    }

    private string GetFacilityFlag(string? location) => location?.ToLower() switch
    {
        "italy" => "🇮🇹",
        "brasil" => "🇧🇷",
        "vietnam" => "🇻🇳",
        _ => "🌍"
    };
}
