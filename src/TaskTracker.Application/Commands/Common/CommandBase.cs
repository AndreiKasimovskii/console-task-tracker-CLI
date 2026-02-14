using TaskTracker.Domain.Abstractions;

namespace TaskTracker.Application.Commands.Common;

abstract class CommandBase(string commandName) : ICommand
{
  protected ICommandParameter[]? Parameters { get; init; }

  public abstract Task Execute(IUserTaskService userTaskService, IPresenter presenter);

  protected internal void ParseParameters(IDictionary<string, string?> args)
  {
    if (Parameters is null)
      throw new Exception("Не инициализированы параметры команды");
    
    if (args.Keys.Except(Parameters.Select(p => p.Name)).Any())
      throw new CommandParseException("Указаны неизвестные параметры для команды.", commandName);
    var requiredParameters = Parameters
        .Where(p => p.Required)
        .ToArray();
    foreach (var reqParameter in requiredParameters)
    {
      if (!args.TryGetValue(reqParameter.Name, out var value))
        throw new CommandParseException($"Не указан обязательный параметр {reqParameter.Name}");
      if (value is null)
        throw new CommandParseException(
            $"Не указано значение для обязательного параметра {reqParameter.Name}");
      reqParameter.SetValue(value);
    }

    var optionalParameters = Parameters.Except(requiredParameters);
    foreach (var optionalParameter in optionalParameters)
    {
      if (!args.TryGetValue(optionalParameter.Name, out var value)) continue;

      if (optionalParameter.IsFlag)
        optionalParameter.SetValue("true");
      else
      {
        if (value is null)
          throw new CommandParseException(
              $"Не указано значение для параметра {optionalParameter.Name}");
        optionalParameter.SetValue(value);
      }
    }
  }
}
