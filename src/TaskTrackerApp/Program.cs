using TaskTrackerApp.Commands;
using static System.Console;

var commands = new Dictionary<string, ICommand>();
WriteLine("Task Tracker (CLI)\n");

StartApplication(commands.Values);

void StartApplication(IReadOnlyCollection<ICommand> commands)
{
    WriteLine("Меню команд:");
    foreach (var command in commands)
    {
        WriteLine($"\t{command.Name} - {command.Description}");
    }
}
