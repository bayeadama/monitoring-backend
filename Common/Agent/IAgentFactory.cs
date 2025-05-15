namespace Common;

public interface IAgentFactory
{
    Task<IAgent> Create(AgentConfig agentInitializerConfig);
}