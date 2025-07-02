using CoffeeMekMonitoringServer.Models;
using CoffeeMekMonitoringServer.Services.Interfaces;

namespace CoffeeMekMonitoringServer.Services.ApiClients;

public class FakeTelemetryApiClient : ITelemetryService
{
    private readonly Random _random = new();
    private readonly ILogger<FakeTelemetryApiClient> _logger;

    public FakeTelemetryApiClient(ILogger<FakeTelemetryApiClient> logger)
    {
        _logger = logger;
    }

    public async Task<ApiResponse<List<MachineTelemetry>>> GetTelemetryByMachineAsync(int machineId, int hours = 24)
    {
        await Task.Delay(500);
        
        try
        {
            var telemetryData = GenerateTelemetryForMachine(machineId, hours);
            _logger.LogDebug("Generated {Count} telemetry records for machine {MachineId}", telemetryData.Count, machineId);
            
            return ApiResponse<List<MachineTelemetry>>.SuccessResult(telemetryData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating telemetry for machine {MachineId}", machineId);
            return ApiResponse<List<MachineTelemetry>>.ErrorResult($"Errore generazione telemetria: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<MachineTelemetry>>> GetTelemetryByLotAsync(int lotId)
    {
        await Task.Delay(400);
        
        try
        {
            var telemetryData = GenerateTelemetryForLot(lotId);
            return ApiResponse<List<MachineTelemetry>>.SuccessResult(telemetryData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating telemetry for lot {LotId}", lotId);
            return ApiResponse<List<MachineTelemetry>>.ErrorResult($"Errore telemetria lotto: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<MachineTelemetry>>> GetRecentTelemetryAsync(int minutes = 30)
    {
        await Task.Delay(300);
        
        try
        {
            var telemetryData = GenerateRecentTelemetry(minutes);
            return ApiResponse<List<MachineTelemetry>>.SuccessResult(telemetryData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recent telemetry");
            return ApiResponse<List<MachineTelemetry>>.ErrorResult($"Errore telemetria recente: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Dictionary<string, object>>> GetMachineDashboardDataAsync(int machineId)
    {
        await Task.Delay(600);
        
        try
        {
            var dashboardData = GenerateMachineDashboard(machineId);
            return ApiResponse<Dictionary<string, object>>.SuccessResult(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating machine dashboard for {MachineId}", machineId);
            return ApiResponse<Dictionary<string, object>>.ErrorResult($"Errore dashboard macchina: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Dictionary<string, object>>> GetFacilityDashboardDataAsync(int facilityId)
    {
        await Task.Delay(700);
        
        try
        {
            var dashboardData = GenerateFacilityDashboard(facilityId);
            return ApiResponse<Dictionary<string, object>>.SuccessResult(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating facility dashboard for {FacilityId}", facilityId);
            return ApiResponse<Dictionary<string, object>>.ErrorResult($"Errore dashboard sede: {ex.Message}");
        }
    }

    // Metodi di generazione dati fake secondo il PDF CoffeeMek
    private List<MachineTelemetry> GenerateTelemetryForMachine(int machineId, int hours)
    {
        var telemetryList = new List<MachineTelemetry>();
        var machineType = GetMachineType(machineId);
        var facilityId = GetFacilityForMachine(machineId);
        var timeZone = GetTimeZoneForFacility(facilityId);
        
        // Genera dati ogni 10 minuti per le ore richieste
        for (int i = 0; i < hours * 6; i++)
        {
            var timestamp = DateTime.UtcNow.AddMinutes(-i * 10);
            
            var telemetry = new MachineTelemetry
            {
                Id = i + 1,
                MachineId = machineId,
                Machine = CreateMachine(machineId, machineType, facilityId),
                LotId = _random.Next(1, 4),
                Timestamp = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                TimeZone = timeZone,
                Data = GenerateDataForMachineType(machineType, timestamp)
            };
            
            telemetryList.Add(telemetry);
        }

        return telemetryList.OrderByDescending(t => t.Timestamp).ToList();
    }

    private List<MachineTelemetry> GenerateTelemetryForLot(int lotId)
    {
        var telemetryList = new List<MachineTelemetry>();
        
        // Workflow CoffeeMek: Fresa → Tornio → Assemblaggio → Test
        var workflowPhases = new[]
        {
            new { Type = "fresa", MachineId = 1, Hours = -8 },
            new { Type = "tornio", MachineId = 2, Hours = -6 },
            new { Type = "assemblaggio", MachineId = 3, Hours = -4 },
            new { Type = "test", MachineId = 4, Hours = -2 }
        };

        foreach (var phase in workflowPhases)
        {
            var facilityId = GetFacilityForMachine(phase.MachineId);
            var timestamp = DateTime.UtcNow.AddHours(phase.Hours);
            
            var telemetry = new MachineTelemetry
            {
                Id = phase.MachineId,
                MachineId = phase.MachineId,
                Machine = CreateMachine(phase.MachineId, phase.Type, facilityId),
                LotId = lotId,
                Lot = new Lot { Id = lotId, Code = $"LOT-{lotId:D3}", Description = $"Lotto CoffeeMek {lotId}" },
                Timestamp = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                TimeZone = GetTimeZoneForFacility(facilityId),
                Data = GenerateDataForMachineType(phase.Type, timestamp)
            };
            
            telemetryList.Add(telemetry);
        }

        return telemetryList;
    }

    private List<MachineTelemetry> GenerateRecentTelemetry(int minutes)
    {
        var telemetryList = new List<MachineTelemetry>();
        var machineTypes = new[] { "fresa", "tornio", "assemblaggio", "test" };
        
        // Genera dati per ogni tipo di macchina ogni 5 minuti
        for (int i = 0; i < minutes / 5; i++)
        {
            foreach (var (machineType, machineId) in machineTypes.Select((type, id) => (type, id + 1)))
            {
                var facilityId = GetFacilityForMachine(machineId);
                var timestamp = DateTime.UtcNow.AddMinutes(-i * 5);
                
                var telemetry = new MachineTelemetry
                {
                    Id = i * 4 + machineId,
                    MachineId = machineId,
                    Machine = CreateMachine(machineId, machineType, facilityId),
                    LotId = _random.Next(1, 4),
                    Timestamp = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    TimeZone = GetTimeZoneForFacility(facilityId),
                    Data = GenerateDataForMachineType(machineType, timestamp)
                };
                
                telemetryList.Add(telemetry);
            }
        }

        return telemetryList.OrderByDescending(t => t.Timestamp).ToList();
    }

    // Generazione dati specifici per tipo macchina secondo PDF CoffeeMek
    private Dictionary<string, object> GenerateDataForMachineType(string machineType, DateTime timestamp)
    {
        return machineType.ToLower() switch
        {
            "fresa" => GenerateDataFresaCNC(),
            "tornio" => GenerateDataTornioAutomatico(),
            "assemblaggio" => GenerateDataLineaAssemblaggio(),
            "test" => GenerateDataLineaTest(),
            _ => new Dictionary<string, object> { ["status"] = "unknown" }
        };
    }

    // Fresa CNC: Codice lotto, tempo ciclo, profondità taglio, vibrazione, allarmi utensile
    private Dictionary<string, object> GenerateDataFresaCNC()
    {
        return new Dictionary<string, object>
        {
            ["codiceLotto"] = $"CF-{_random.Next(1000, 9999)}",
            ["tempoCiclo"] = _random.Next(45, 180), // secondi
            ["profonditaTaglio"] = Math.Round(_random.NextDouble() * 8 + 2, 2), // mm
            ["vibrazione"] = Math.Round(_random.NextDouble() * 0.8 + 0.1, 3), // g
            ["allarmiUtensile"] = _random.Next(0, 3),
            ["statusOperativo"] = _random.Next(0, 10) < 9 ? "operative" : "maintenance",
            ["temperaturaMandrino"] = _random.Next(35, 75), // °C
            ["velocitaRotazione"] = _random.Next(1000, 3000), // RPM
            ["consumoEnergetico"] = Math.Round(_random.NextDouble() * 12 + 8, 2) // kWh
        };
    }

    // Tornio automatico: Stato macchina, velocità rotazione, temperatura mandrino, numero pezzi completati
    private Dictionary<string, object> GenerateDataTornioAutomatico()
    {
        return new Dictionary<string, object>
        {
            ["statoMacchina"] = _random.Next(0, 10) < 9 ? "operative" : "offline",
            ["velocitaRotazione"] = _random.Next(800, 2800), // RPM
            ["temperaturaMandrino"] = _random.Next(40, 85), // °C
            ["numeroPezziCompletati"] = _random.Next(15, 45),
            ["consumoEnergetico"] = Math.Round(_random.NextDouble() * 18 + 10, 2), // kWh
            ["pressione"] = Math.Round(_random.NextDouble() * 3 + 5, 1), // bar
            ["precisione"] = Math.Round(_random.NextDouble() * 0.05 + 0.01, 3), // mm
            ["durataLavorazione"] = _random.Next(120, 300) // secondi
        };
    }

    // Linea assemblaggio: Lotto associato, tempo medio per stazione, numero operatori, anomalie rilevate
    private Dictionary<string, object> GenerateDataLineaAssemblaggio()
    {
        return new Dictionary<string, object>
        {
            ["lottoAssociato"] = $"ASM-{_random.Next(100, 999)}",
            ["tempoMedioStazione"] = _random.Next(180, 420), // secondi
            ["numeroOperatori"] = _random.Next(3, 8),
            ["anomalieRilevate"] = _random.Next(0, 4),
            ["produttivita"] = Math.Round(_random.NextDouble() * 25 + 75, 1), // %
            ["qualita"] = Math.Round(_random.NextDouble() * 8 + 92, 1), // %
            ["tempoTotale"] = _random.Next(1200, 2400), // secondi
            ["pezziAssemblati"] = _random.Next(8, 25)
        };
    }

    // Linea di test: Esiti test funzionali, pressione e temperatura caldaia, consumo energetico
    private Dictionary<string, object> GenerateDataLineaTest()
    {
        var testPassed = _random.Next(0, 10) < 8;
        
        return new Dictionary<string, object>
        {
            ["esitiTestFunzionali"] = testPassed ? "PASS" : "FAIL",
            ["pressioneCaldaia"] = Math.Round(_random.NextDouble() * 3 + 7, 1), // bar
            ["temperaturaCaldaia"] = _random.Next(88, 108), // °C
            ["consumoEnergetico"] = Math.Round(_random.NextDouble() * 8 + 3, 2), // kWh
            ["durataTesting"] = _random.Next(300, 1200), // secondi
            ["numeroTestEseguiti"] = _random.Next(5, 15),
            ["testSuperati"] = testPassed ? _random.Next(8, 15) : _random.Next(3, 8),
            ["temperaturaAcqua"] = _random.Next(20, 30), // °C
            ["pressionePompa"] = Math.Round(_random.NextDouble() * 2 + 4, 1) // bar
        };
    }

    private Dictionary<string, object> GenerateMachineDashboard(int machineId)
    {
        var machineType = GetMachineType(machineId);
        var facilityId = GetFacilityForMachine(machineId);
        
        return new Dictionary<string, object>
        {
            ["machineId"] = machineId,
            ["machineType"] = machineType,
            ["facilityId"] = facilityId,
            ["status"] = _random.Next(0, 10) < 8 ? "operative" : "maintenance",
            ["efficiency"] = Math.Round(_random.NextDouble() * 20 + 80, 1),
            ["dailyProduction"] = _random.Next(20, 80),
            ["qualityRate"] = Math.Round(_random.NextDouble() * 8 + 92, 1),
            ["lastMaintenance"] = DateTime.UtcNow.AddDays(-_random.Next(1, 30)).ToString("yyyy-MM-dd"),
            ["nextMaintenance"] = DateTime.UtcNow.AddDays(_random.Next(1, 20)).ToString("yyyy-MM-dd"),
            ["currentLot"] = $"LOT-{_random.Next(1, 100):D3}",
            ["alerts"] = _random.Next(0, 5),
            ["energyConsumption"] = Math.Round(_random.NextDouble() * 25 + 10, 2),
            ["workflowPhase"] = GetWorkflowPhase(machineType)
        };
    }

    private Dictionary<string, object> GenerateFacilityDashboard(int facilityId)
    {
        var facilityData = GetFacilityInfo(facilityId);
        
        return new Dictionary<string, object>
        {
            ["facilityId"] = facilityId,
            ["facilityName"] = facilityData.Name,
            ["location"] = facilityData.Location,
            ["timezone"] = facilityData.TimeZone,
            ["localTime"] = GetLocalTime(facilityData.TimeZone),
            ["activeMachines"] = _random.Next(8, 16),
            ["totalMachines"] = 16,
            ["dailyProduction"] = _random.Next(150, 400),
            ["monthlyTarget"] = _random.Next(4000, 8000),
            ["efficiency"] = Math.Round(_random.NextDouble() * 15 + 80, 1),
            ["qualityRate"] = Math.Round(_random.NextDouble() * 6 + 94, 1),
            ["activeLots"] = _random.Next(8, 20),
            ["completedToday"] = _random.Next(25, 85),
            ["alerts"] = _random.Next(0, 8),
            ["energyConsumption"] = Math.Round(_random.NextDouble() * 150 + 200, 2),
            ["currentShift"] = GetCurrentShift(),
            ["shiftProduction"] = _random.Next(40, 120),
            ["workflowStatus"] = GenerateWorkflowStatus()
        };
    }

    // Helper methods
    private string GetMachineType(int machineId) => (machineId % 4) switch
    {
        1 => "fresa",
        2 => "tornio", 
        3 => "assemblaggio",
        0 => "test",
        _ => "unknown"
    };

    private int GetFacilityForMachine(int machineId) => (machineId % 3) + 1;

    private string GetTimeZoneForFacility(int facilityId) => facilityId switch
    {
        1 => "+1", // Milano
        2 => "-3", // São Paulo
        3 => "+7", // Ho Chi Minh
        _ => "+0"
    };

    private Machine CreateMachine(int machineId, string type, int facilityId)
    {
        return new Machine
        {
            Id = machineId,
            Name = $"CoffeeMek {type.ToUpper()} #{machineId:D2}",
            Type = type,
            FacilityId = facilityId,
            Status = _random.Next(0, 10) < 8 ? "operative" : "maintenance"
        };
    }

    private (string Name, string Location, string TimeZone) GetFacilityInfo(int facilityId) => facilityId switch
    {
        1 => ("Stabilimento Milano", "Italy", "+1"),
        2 => ("Stabilimento São Paulo", "Brasil", "-3"),
        3 => ("Stabilimento Ho Chi Minh", "Vietnam", "+7"),
        _ => ("Unknown Facility", "Unknown", "+0")
    };

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

    private string GetCurrentShift()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            >= 6 and < 14 => "Turno Mattino (06:00-14:00)",
            >= 14 and < 22 => "Turno Pomeriggio (14:00-22:00)",
            _ => "Turno Notte (22:00-06:00)"
        };
    }

    private string GetWorkflowPhase(string machineType) => machineType switch
    {
        "fresa" => "Fase 1: Lavorazione CNC",
        "tornio" => "Fase 2: Tornitura Automatica",
        "assemblaggio" => "Fase 3: Assemblaggio Componenti",
        "test" => "Fase 4: Test Funzionali",
        _ => "Fase Sconosciuta"
    };

    private Dictionary<string, object> GenerateWorkflowStatus()
    {
        return new Dictionary<string, object>
        {
            ["fresa"] = new { Active = _random.Next(2, 6), Total = 6, Efficiency = _random.Next(80, 95) },
            ["tornio"] = new { Active = _random.Next(2, 5), Total = 5, Efficiency = _random.Next(85, 98) },
            ["assemblaggio"] = new { Active = _random.Next(1, 4), Total = 3, Efficiency = _random.Next(75, 90) },
            ["test"] = new { Active = _random.Next(1, 3), Total = 2, Efficiency = _random.Next(90, 99) }
        };
    }
}
