namespace Common.Models;

public record Command(Guid RequestId, string Name, string[] Arguments);