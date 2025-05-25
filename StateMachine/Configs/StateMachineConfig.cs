namespace StateMachine.Configs;

public class StateMachineConfig
{
    public string InitialStateName { get; set; }
    public Dictionary<string, List<StateMachineTriggerConfig>> StateTransitions { get; set; } 
        = new Dictionary<string, List<StateMachineTriggerConfig>>();
}