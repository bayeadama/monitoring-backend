using System.Text.Json.Serialization;

namespace Presentation.Orchestrator.Configurations;

public class ApplicationsConfigurations
{
    [JsonPropertyName("applications")]
    public List<ApplicationConfiguration> Applications { get; set; }
}

public class ApplicationConfiguration
{
    
[JsonPropertyName("applicationId")]
    public string ApplicationId { get; set; }

    [JsonPropertyName("applicationShortName")]
    public string ApplicationShortName { get; set; }

    [JsonPropertyName("applicationName")]
    public string ApplicationName { get; set; }
    
    [JsonPropertyName("agents")]
    public string[] Agents { get; set; } = [];

    [JsonPropertyName("ui")]
    public ApplicationConfigurationUserInterface UserInterface { get; set; }
    
    [JsonPropertyName("workflow")]
    public ApplicationConfigurationWorkflow Workflow { get; set; } 

}


public class ApplicationConfigurationUserInterface
{
    [JsonPropertyName("groupsToPrioritize")]
    public List<string> GroupsToPrioritize { get; set; } = [];

    [JsonPropertyName("translations")]
    public Dictionary<string, string> Translations { get; set; } = new();
    
   
}

public class ApplicationConfigurationWorkflow
{


    [JsonPropertyName("type")]
    public string WorkflowType { get; set; }

    [JsonPropertyName("steps-waiting-times")]
    public Dictionary<string, int> StepsWaitingTimes { get; set; }
}
