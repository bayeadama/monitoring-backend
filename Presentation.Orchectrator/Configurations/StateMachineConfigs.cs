using Domain.Model;
using Presentation.Orchestrator.Constants;
using StateMachine.Configs;

namespace Presentation.Orchestrator.Configurations;

public static class StateMachineConfigs
{
    
    public static StateMachineConfig GetStateMachineConfig(StateMachineType stateMachineType)
    {
        return stateMachineType switch
        {
            StateMachineType.Standard => GetStandardWorkflowConfig(),
            _ => throw new ArgumentOutOfRangeException(nameof(stateMachineType), stateMachineType, null)
        };
    }
    
    
    private static StateMachineConfig GetStandardWorkflowConfig()
    {
            StateMachineConfig stateMachineConfig = new StateMachineConfig
            {
                InitialStateName = WorkflowStates.InitiatedState,
                StateTransitions = new Dictionary<string, List<StateMachineTriggerConfig>>
                {
                    //Initial state
                    {
                        WorkflowStates.InitiatedState, BuildInitiatedStateTransitions()
                    },
                
                    //'Who Am I' requested
                    {
                        WorkflowStates.WhoAmIRequestedState, BuildWhoAmIRequestedStateTransitions()
                    },
                
                    //'Who Am I' completed
                    {
                        WorkflowStates.WhoAmICompletedState, BuildWhoAmICompletedStateTransitions()
                    },
                
                    //'Analysis' requested
                    {
                        WorkflowStates.AnalysisRequestedState, BuildAnalysisRequestedStateTransitions()
                    },
                
                    //'Analysis' completed
                    {
                        WorkflowStates.AnalysisCompletedState, BuildAnalysisCompletedStateTransitions()
                    }
                }
            };
            return stateMachineConfig;
    }

    private static List<StateMachineTriggerConfig> BuildInitiatedStateTransitions()
    {
        return new List<StateMachineTriggerConfig>
        {
            new()
            {
                NextState = WorkflowStates.WhoAmIRequestedState,
                TriggerName = TriggerNames.RequestWhoAmiAction
            }
        };
    }
    
    private static List<StateMachineTriggerConfig> BuildWhoAmIRequestedStateTransitions()
    {
        return new List<StateMachineTriggerConfig>
        {
            new()
            {
                NextState = WorkflowStates.WhoAmICompletedState,
                TriggerName = TriggerNames.WhoAmiCompletedAction
            }
        };
    }
    
    private static List<StateMachineTriggerConfig> BuildWhoAmICompletedStateTransitions()
    {
        return new List<StateMachineTriggerConfig>
        {
            new()
            {
                NextState = WorkflowStates.WhoAmIRequestedState,
                TriggerName = TriggerNames.RequestWhoAmiAction
            },
            new()
            {
                NextState = WorkflowStates.AnalysisRequestedState,
                TriggerName = TriggerNames.RequestAnalysisAction
            }
        };
    }
    
    private static List<StateMachineTriggerConfig> BuildAnalysisRequestedStateTransitions()
    {
        return new List<StateMachineTriggerConfig>
        {
            new()
            {
                NextState = WorkflowStates.AnalysisCompletedState,
                TriggerName = TriggerNames.AnalysisCompletedAction
            }
        };
    }
    
    private static List<StateMachineTriggerConfig> BuildAnalysisCompletedStateTransitions()
    {
        return new List<StateMachineTriggerConfig>
        {
            new()
            {
                NextState = WorkflowStates.WhoAmIRequestedState,
                TriggerName = TriggerNames.RequestWhoAmiAction
            }
        };
    }
}