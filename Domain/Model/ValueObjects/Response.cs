namespace Domain.Model.ValueObjects;

public class Response
{
    public string FromAgent { get; set; }
    public string ApplicationTrigram { get; set; }
    public Command FromCommand { get; set; }
    public string Payload { get; set; }
}