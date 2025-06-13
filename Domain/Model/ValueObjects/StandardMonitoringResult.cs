namespace Domain.Model.ValueObjects;

public class StandardMonitoringResult
{
    public string ApplicationTrigram { get; set; }
    public string ComponentName { get; set; }

    public string Checker { get; set; }
    public bool ResultOk { get; set; }
    public string OkMessage { get; set; }
    public string ErrorMessage { get; set; }
}