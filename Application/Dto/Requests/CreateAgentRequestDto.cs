using Domain.Model;

namespace Application.Dto.Requests;

public class CreateAgentRequestDto
{
    public string ApplicationTrigram { get; set; }
    public string ComponentName { get; set; }
    public string AgentId { get; set; }

    public AgentType AgentType { get; set; }

    public string CommanderMailBox { get; set; }
    public string ResponseMailBox { get; set; }
}