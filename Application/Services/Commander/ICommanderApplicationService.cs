using Domain.Model.ValueObjects;

namespace Application.Services.Commander;

public interface ICommanderApplicationService
{
    /// <summary>
    /// Initializes a commander
    /// </summary>
    /// <param name="commanderName"></param>
    /// <returns></returns>
    Task<Domain.Model.Commander> InitializeCommanderAsync(string commanderName);

    /// <summary>
    /// Registers a response's handler for the specified commander.
    /// </summary>
    /// <param name="commander"></param>
    /// <param name="responseHandler"></param>
    /// <returns></returns>
    Task RegisterResponseHandlerAsync(Domain.Model.Commander commander,
        Action<Domain.Model.Listener, Domain.Model.ValueObjects.Response> responseHandler);

    /// <summary>
    /// Publishes a command from the commander to the listener(s).
    /// </summary>
    /// <param name="commander"></param>
    /// <param name="sourceCommand"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    Task PublishCommandAsync(Domain.Model.Commander commander, Command sourceCommand, string destination);
}