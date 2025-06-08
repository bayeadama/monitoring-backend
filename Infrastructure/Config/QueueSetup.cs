namespace Infrastructure.Config;


public class QueueSetup
{
    public string QueueName { get; set; }
    public string ExchangeName { get; set; }
    public List<string> RoutingKeys { get; set; }

    public Dictionary<string, object>? Arguments { get; set; }

    public void AddArgument(string key, object value)
    {
        Arguments ??= new Dictionary<string, object>();
        Arguments.Add(key, value);
    }
}