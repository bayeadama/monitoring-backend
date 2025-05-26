using FluentAssertions;
using StateMachine.Configs;
using StateMachine.Tests.Constants;
using StateMachine.Workflow;

namespace StateMachine.Tests;

public class WorkflowTests
{
    IWorkflow _workflow;


    public void InitializeWorkflow(string? initialState = null)
    {
        StateMachineConfig stateMachineConfig = BuildStateMachineConfig(initialState);
        IWorkflow workflow = new DefaultWorkflow(stateMachineConfig);
        _workflow = workflow;
    }


    [Theory]
    [InlineData(null, TriggerNames.RequestWhoAmiAction, WorkflowStates.WhoAmIRequestedState)]
    [InlineData(WorkflowStates.WhoAmIRequestedState, TriggerNames.WhoAmiCompletedAction, WorkflowStates.WhoAmICompletedState)]
    [InlineData(WorkflowStates.WhoAmICompletedState, TriggerNames.RequestWhoAmiAction, WorkflowStates.WhoAmIRequestedState)]
    [InlineData(WorkflowStates.WhoAmICompletedState, TriggerNames.RequestAnalysisAction, WorkflowStates.AnalysisRequestedState)]
    [InlineData(WorkflowStates.AnalysisRequestedState, TriggerNames.AnalysisCompletedAction, WorkflowStates.AnalysisCompletedState)]
    [InlineData(WorkflowStates.AnalysisCompletedState, TriggerNames.RequestWhoAmiAction, WorkflowStates.WhoAmIRequestedState)]
    
    public void ApplyAction_WhenTriggerExistingAction_ThenReturnAssociatedState(string initialState, string triggerName, string expectedState)
    {
        //Arrange
        InitializeWorkflow(initialState);

        //Act
        var result = _workflow.ApplyAction(triggerName);

        //Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedState);
    }


    private StateMachineConfig BuildStateMachineConfig(string? initialState = null)
    {
        StateMachineConfig stateMachineConfig = new StateMachineConfig
        {
            InitialStateName = initialState ?? WorkflowStates.InitiatedState,
            StateTransitions = new Dictionary<string, List<StateMachineTriggerConfig>>
            {
                //Initial state
                {
                    WorkflowStates.InitiatedState, [
                        new()
                        {
                            NextState = WorkflowStates.WhoAmIRequestedState,
                            TriggerName = TriggerNames.RequestWhoAmiAction
                        }
                    ]
                },
                
                //'Who Am I' requested
                {
                    WorkflowStates.WhoAmIRequestedState, [
                        new()
                        {
                            NextState = WorkflowStates.WhoAmICompletedState,
                            TriggerName = TriggerNames.WhoAmiCompletedAction
                        }
                    ]
                },
                
                //'Who Am I' completed
                {
                    WorkflowStates.WhoAmICompletedState, [
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
                    ]
                },
                
                //'Analysis' requested
                {
                    WorkflowStates.AnalysisRequestedState, [
                        new()
                        {
                            NextState = WorkflowStates.AnalysisCompletedState,
                            TriggerName = TriggerNames.AnalysisCompletedAction
                        }
                    ]
                },
                
                //'Analysis' completed
                {
                    WorkflowStates.AnalysisCompletedState, [
                        new()
                        {
                            NextState = WorkflowStates.WhoAmIRequestedState,
                            TriggerName = TriggerNames.RequestWhoAmiAction
                        }
                    ]
                }
            }
        };

        return stateMachineConfig;
    }
}