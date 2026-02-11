namespace TaskTracker.Application;

class CommandParseException(string message, string? commandName = null) : Exception(message)
{
  public string? CommandName { get; } = commandName;
}