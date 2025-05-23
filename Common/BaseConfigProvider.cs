namespace Common;

public abstract class BaseConfigProvider
{
    private readonly string _configurationSection;

    protected BaseConfigProvider(string configurationSection)
    {
        _configurationSection = configurationSection ?? throw new ArgumentNullException(nameof(configurationSection));;
    }

    protected string GetConfigKey(string key) => $"{_configurationSection}:{key}";

}