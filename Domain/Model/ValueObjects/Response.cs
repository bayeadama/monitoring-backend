namespace Domain.Model.ValueObjects;

public class Response
{
    public Command FromCommand { get; set; }
    public string Payload { get; set; }
}