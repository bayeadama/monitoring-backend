namespace StateMachine.Configs;

public class StateMachineTriggerConfig
{
    public string TriggerName { get; set; }

    public string NextState { get; set; }
    
    public Func<string, bool> PreCondition { get; set; }
    
}