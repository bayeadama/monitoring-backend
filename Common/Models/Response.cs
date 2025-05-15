namespace Common.Models;

public record Response(Guid RequestId, string AgentId, string CommandName, string Content);