namespace Common.Commander;

public interface ICommanderFactory
{
    Task<ICommander> Create(CommanderConfig commanderConfig);
}