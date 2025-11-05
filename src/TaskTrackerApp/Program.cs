using static System.Console;

WriteLine("Task Tracker (CLI)\n");

if (args.Length == 0)
{
    WriteLine("Использование: tasktrackerapp [command]");

    var commands = GetAllCommands();
    WriteLine("Команды (commands):");
    foreach(var command in commands)
    {
        WriteLine($"\t{command.Name}\t{command.Description}");
    }
}

Command[] GetAllCommands() => [];

record Command(string Name, string? Description);