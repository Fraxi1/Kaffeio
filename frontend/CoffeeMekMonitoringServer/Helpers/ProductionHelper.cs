namespace CoffeeMekMonitoringServer.Helpers;

public static class ProductionHelper
{
    public static string GetFacilityFlag(string? location) => location?.ToLower() switch
    {
        "italy" => "🇮🇹",
        "brasil" => "🇧🇷",
        "vietnam" => "🇻🇳",
        _ => "🌍"
    };

    public static string GetPhaseIcon(string? phase) => phase?.ToLower() switch
    {
        "fresa" => "fas fa-cog",
        "tornio" => "fas fa-circle-notch",
        "assemblaggio" => "fas fa-puzzle-piece",
        "test" => "fas fa-vial",
        _ => "fas fa-question"
    };

    public static string GetStatusBadge(string? status) => status?.ToLower() switch
    {
        "scheduled" => "secondary",
        "inprogress" => "primary",
        "completed" => "success",
        "delayed" => "danger",
        _ => "secondary"
    };

    public static string GetStatusIcon(string? status) => status?.ToLower() switch
    {
        "scheduled" => "📅",
        "inprogress" => "⚡",
        "completed" => "✅",
        "delayed" => "⏰",
        _ => "❓"
    };

    public static string GetProgressColor(int progress) => progress switch
    {
        < 25 => "danger",
        < 50 => "warning",
        < 75 => "info",
        < 100 => "primary",
        _ => "success"
    };

    public static string GetLocalTime(string timeZone)
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

    public static string FormatDate(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString)) return "N/A";
        
        try
        {
            var date = DateTime.Parse(dateString);
            return date.ToString("dd/MM/yyyy");
        }
        catch
        {
            return dateString;
        }
    }

    public static bool IsOverdue(string? endDate)
    {
        if (string.IsNullOrEmpty(endDate)) return false;
        
        try
        {
            var date = DateTime.Parse(endDate);
            return date < DateTime.Now;
        }
        catch
        {
            return false;
        }
    }
}
