using System.Timers;

namespace Presentation.Orchestrator;

public static class ProgrammedTask
{
    public static Task ExecuteAsync(Action action, int timeInMilliseconds)
    {
        var timer = new System.Timers.Timer(timeInMilliseconds);
        timer.Elapsed += (source, e) =>
        {
            // Execute the action
            action();

            // Disable the timer after execution
            timer.Enabled = false;
        };

        // AutoReset true: keeps firing repeatedly
        timer.AutoReset = false;

        // Enable the timer
        timer.Enabled = true;

        return Task.CompletedTask;
    }
}