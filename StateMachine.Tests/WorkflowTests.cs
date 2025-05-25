using FluentAssertions;
using StateMachine.Configs;
using StateMachine.Tests.Constants;
using StateMachine.Workflow;

namespace StateMachine.Tests;

public class WorkflowTests
{
    IWorkflow _workflow;


    public WorkflowTests()
    {
        StateMachineConfig stateMachineConfig = BuildStateMachineConfig();
        IWorkflow workflow = new DefaultWorkflow(stateMachineConfig);
        _workflow = workflow;
    }


    [Fact]
    public void ApplyAction_WhenTriggerExistingAction_ThenReturnAssociatedState()
    {
        //Arrange

        //Act
        var result = _workflow.ApplyAction(TriggerNames.RequestWhoAmiAction);

        //Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(WorkflowStates.WhoAmIRequestedState);
    }


    private StateMachineConfig BuildStateMachineConfig()
    {
        StateMachineConfig stateMachineConfig = new StateMachineConfig
        {
            InitialStateName = WorkflowStates.InitiatedState,
            StateTransitions = new Dictionary<string, List<StateMachineTriggerConfig>>
            {
                {
                    WorkflowStates.InitiatedState, [
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