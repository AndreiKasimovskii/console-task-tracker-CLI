using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Services;
using TaskTracker.Infrastructure;

namespace TaskTracker.Application;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var store = new FileStore();
        IUserTaskRepository repository = new UserTaskRepository(store);
        IUserTaskService service = new UserTaskService(repository);
        var dispatcher = new CommandDispatcher(new ConsolePresenter(), service);
        await dispatcher.Run(args);
    }
}