using System.Text;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Services;
using TaskTracker.Infrastructure;

namespace TaskTrackerCLI.App;

class CommandDispatcher(IPresenter presenter)
{
  private static readonly Dictionary<string, CommandInfo> CommandInfos = new()
  {
    {"list", new CommandInfo([], [], "list", ListCommand.CreateCommand)}
  };

  public async Task Run(string[] applicationParameters)
  {
    string commandName;
    string[] commandParameters = [];

    switch (applicationParameters.Length)
    {
      case 0:
        presenter.PrintError("Не указана команда для выполнения!");
        return;
      case 1:
        commandName = applicationParameters[0];
        break;
      default:
        commandName = applicationParameters[0];
        commandParameters = applicationParameters[1..];
        break;
    }

    if (!CommandInfos.TryGetValue(commandName, out var commandInfo))
    {
      presenter.PrintError($"Команда {commandName} не определена в приложении.");
      presenter.PrintHint(AllCommandHelp());
      return;
    }

    if (!CommandParametersIsCorrect(commandInfo, commandParameters))
    {
      presenter.PrintError($"Не правильно определены параметры команды {commandName}!");
      presenter.PrintHint(GetCommandHelp(commandName));
      return;
    }

    try
    {
      var command = commandInfo.CommandCreator(new UserTaskService(new UserTaskRepository(new FileStore())),
        presenter, []);
      await command.Execute();
    }
    catch (InvalidDataException exception)
    {
      HandleInvalidDataException(exception);
    }
    catch (TimeoutException)
    {
      presenter.PrintWarning("Операции занята (таймаут)");
    }
    catch (IOException exception)
    {
      presenter.PrintError($"Ошибка ввода/вывода: {exception.Message}");
    }
    catch (Exception exception)
    {
      presenter.PrintError($"Неизвестная ошибка! ({exception.GetType().Name})");
    }
  }

  private void HandleInvalidDataException(InvalidDataException exception)
  {
    presenter.PrintError("Хранилище повереждено!");

    string? input;
    do
    {
      presenter.PrintQuestion("Хотите пересоздать хранилище (Y/N)?");
      input = Console.ReadLine();
    } while (input is not ("Y" or "y" or "N" or "n"));

    if(input is "Y" or "y" )
    {
      ResetStorage();
    }
    else
    {
      presenter.PrintQuestion("Хотите вывести текст ошибки (Y/N)?");
      input = Console.ReadLine();
      if (input is "Y" or "y")
      {
        presenter.PrintError($"Error: {exception.Message}\nInnerException:({exception.InnerException?.GetType()}) {exception.InnerException?.Message}");
      }
    }
  }

  private void ResetStorage()
  {
    var filePath = Path.Combine(Environment.CurrentDirectory, "tasks.json");
    File.Move(filePath, Path.Combine(Environment.CurrentDirectory, $"corruptedStorage_{DateTime.Now.Date}.json"), true);

    if (File.Exists(filePath))
    {
      File.Delete(filePath);
    }

    using var _ = File.Create(filePath);
  }

  private string AllCommandHelp()
    {
      StringBuilder sb = new("Подсказка по всем командам приложения:");
      foreach (var command in CommandInfos)
      {
        sb.AppendLine($"\t{command.Key}: {command.Value.HelpSection}");
      }

      return sb.ToString();
    }

    private string GetCommandHelp(string commandName)
    {
      return string.Format("Подсказка по команде приложения {0}:\n\t{1}:{2}",
        commandName,
        commandName,
        CommandInfos[commandName].HelpSection);
    }

    private bool CommandParametersIsCorrect(CommandInfo commandInfo, string[] commandParameters)
    {
      if (commandInfo.RequiredParameters.Length > commandParameters.Length)
        return false;

      if (commandParameters.Length > commandInfo.RequiredParameters.Length + commandInfo.OptionalParameters.Length)
        return false;

      return true;
    }

    private record CommandInfo(CommandParameter[] RequiredParameters, CommandParameter[] OptionalParameters, string HelpSection,
    Func<IUserTaskService, IPresenter, CommandParameter[], ICommand> CommandCreator);
}