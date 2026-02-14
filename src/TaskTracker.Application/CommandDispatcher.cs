using System.Text;
using TaskTracker.Application.Commands.Common;
using TaskTracker.Domain.Abstractions;

namespace TaskTracker.Application;

internal class CommandDispatcher(IPresenter presenter, IUserTaskService userTaskService)
{
  private static readonly Dictionary<string, CommandInfo> CommandInfos = new()
  {
    {"list", new CommandInfo(
      "list " +
      "\nКоманда выводит список активных задач на текущую дату. Задачи отсортированы по сроку выполнения.")},
    {"add", new CommandInfo(
      "add --title <title> [--desc <description>] [--deadline <deadline(yyyy-MM-dd)>]" +
      "\nКоманда создает новую задачу с заголовком <title>, описанием <description> и сроком <deadline>." +
      "Параметры --desc и -deadline не обязательные.")}
  };

  public async Task Run(string[] applicationParameters)
  {
    try
    {
      var (commandName, commandArguments) = ParseCliArguments(applicationParameters);
      var command = CommandFactory.Create(commandName, commandArguments);
      await command.Execute(userTaskService, presenter);
    }
    catch (InvalidDataException exception)
    {
      HandleInvalidDataException(exception);
    }
    catch (TimeoutException)
    {
      presenter.PrintWarning("Операции не выполнена. Превышено время ожидания");
    }
    catch (IOException exception)
    {
      presenter.PrintError($"Ошибка ввода/вывода: {exception.Message}");
    }
    catch (CommandParseException exception)
    {
      presenter.PrintError($"Ошибка парсинга команды: {exception.Message}");
      presenter.PrintHint(exception.CommandName is not null 
        ? GetCommandHelp(exception.CommandName) 
        : AllCommandHelp()
      );
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

    if (input is "Y" or "y")
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
    StringBuilder sb = new("Подсказка по всем командам приложения:\n");
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

  private (string CommandName, Dictionary<string, string?> Parameters) ParseCliArguments(string[] args)
  {
    var commandName = args.Length > 0
      ? args[0]
      : throw new CommandParseException("Не указана команда для выполнения!");

    // if (!CommandInfos.TryGetValue(commandName, out var commandInfo))
    //   throw new CommandParseException($"Команда {commandName} не определена в приложении.");

    var commandArgs = PrepareCommandArgs(args);

    // var command = commandInfo.CommandCreator(userTaskService, presenter, commandArgs);

    return (commandName, commandArgs);
  }

  private static Dictionary<string, string?> PrepareCommandArgs(string[] args)
  {
    var commandArgs = new Dictionary<string, string?>();

    if (args.Length <= 1)
      return commandArgs;

    if (!args[1].StartsWith("--"))
      throw new CommandParseException("Неверный формат команды! Команда не может принимать значение без имени аргумента");

    for (var i = 1; i < args.Length;)
    {
      if(!args[i].StartsWith("--"))
        continue;
      
      var paramName = args[i];
      if (!commandArgs.TryAdd(paramName, null))
        throw new CommandParseException($"Параметр {paramName} указан дважды!");
      
      ++i;
      if (i >= args.Length || args[i].StartsWith("--")) 
        continue;
      
      commandArgs[paramName] = args[i];
      ++i;
    }

    return commandArgs;
  }

  private record CommandInfo(string HelpSection);
}