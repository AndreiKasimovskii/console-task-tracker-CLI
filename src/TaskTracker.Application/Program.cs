namespace TaskTrackerCLI.App;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var dispatcher = new CommandDispatcher(new ConsolePresenter());
        await dispatcher.Run(args);
    }
}