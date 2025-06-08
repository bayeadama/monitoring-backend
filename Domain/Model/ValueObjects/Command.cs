namespace Domain.Model.ValueObjects;

public class Command
{
    public Guid RequestId { get; set; } 
    public DateTime CreatedAt { get; set; }
    public CommandName Name { get; set; }
    public string[] Arguments { get; set; }
}