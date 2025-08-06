namespace Domain.Model.ValueObjects;

public class StandardMonitoringResult : MonitoringResult<MonitoringResult>
{
    
}

public enum MonitoringResult
{
    Success,
    Failure,
    Warning
}