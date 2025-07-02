namespace Presentation.Orchestrator.Constants;

public static class WorkflowStates
{
    public const string InitiatedState = "Initiated";
    
    public const string WhoAmIRequestedState = "WhoAmI.Requested";
    public const string WhoAmICompletedState = "WhoAmI.Completed";
    
    public const string AnalysisRequestedState = "Analysis.Requested";
    public const string AnalysisCompletedState = "Analysis.Completed";

}