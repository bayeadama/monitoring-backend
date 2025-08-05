namespace Domain.Model.ValueObjects;

public class StandardMonitoringResult
{
    public string ApplicationTrigram { get; set; }
    public string ComponentName { get; set; }

    public string Category { get; set; }
    public string Checker { get; set; }
    public string Host { get; set; }
    public string AgentId { get; set; }
    public string Group { get; set; }
    public MonitoringResult Result { get; set; }
    public string ResultMessage { get; set; }
}

public enum MonitoringResult
{
    Success,
    Failure,
    Warning
}