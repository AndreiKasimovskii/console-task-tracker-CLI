namespace TaskTracker.Application.Commands.Common;

internal static class CommandFactory
{
  private static readonly Dictionary<string, Func<CommandBase>> Creators = [];

  static CommandFactory()
  {
    Creators["list"] = () => new ListCommand();
    Creators["add"] = () => new AddCommand();
    Creators["del"] = () => new DeleteCommand();
  }

  public static ICommand Create(string commandName, Dictionary<string, string?> parameters)
  {
    if(!Creators.TryGetValue(commandName, out var commandCreator))
      throw new CommandParseException($"Команда {commandName} не определена в приложении.");
    var command = commandCreator();
    command.ParseParameters(parameters);
    return command;
  }
}
