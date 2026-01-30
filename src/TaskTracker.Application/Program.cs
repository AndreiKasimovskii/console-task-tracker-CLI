namespace TaskTracker.Application;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var dispatcher = new CommandDispatcher(new ConsolePresenter());
        await dispatcher.Run(args);
    }
}