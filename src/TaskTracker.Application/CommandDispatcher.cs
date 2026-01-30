using System.Text;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Services;
using TaskTracker.Infrastructure;

namespace TaskTracker.Application;

class CommandDispatcher(IPresenter presenter)
{
  private static readonly Dictionary<string, CommandInfo> CommandInfos = new()
  {
    {"list", new CommandInfo(ListCommand.CreateCommand, 
      "list " +
      "\nКоманда выводит список активных задач на текущую дату. Задачи отсортированы по сроку выполнения.")},
    {"add", new CommandInfo(AddCommand.CreateCommand, 
      "add --title <title> [--desc <description>] [--deadline <deadline(yyyy-MM-dd)>]" +
      "\nКоманда создает новую задачу с заголовком <title>, описанием <description> и сроком <deadline>." +
      "Параметры --desc и -deadline не обязательные.")}
  };

  public async Task Run(string[] applicationParameters)
  {
    try
    {
      var command = ParseCommand(applicationParameters);
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
    catch (CommandParseException exception)
    {
      presenter.PrintError($"Ошибка парсинга команды: {exception.Message}");
      presenter.PrintHint(AllCommandHelp());
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

  private ICommand ParseCommand(string[] args)
  {
    var commandName = args.Length > 0
      ? args[0]
      : throw new CommandParseException("Не указана команда для выполнения!");

    if (!CommandInfos.TryGetValue(commandName, out var commandInfo))
      throw new CommandParseException($"Команда {commandName} не определена в приложении.");

    var commandArgs = PrepareArgs(args);

    var command = commandInfo.CommandCreator(new UserTaskService(new UserTaskRepository(new FileStore())),
      new ConsolePresenter(), commandArgs);

    return command;
  }

  private IDictionary<string, string?> PrepareArgs(string[] args)
  {
    var commandArgs = new Dictionary<string, string?>();

    if (args.Length <= 1)
      return commandArgs;

    if (!args[1].StartsWith("--"))
      throw new CommandParseException("Неверный формат команды! Команда не может принимать значение без имени аргумента");

    for (int i = 1; i < args.Length;)
    {
      string paramName = args[i];
      string? value = null;
      ++i;
      if (i < args.Length 
          && !args[i].StartsWith("--"))
      {
        value = args[i];
        ++i;
      }

      if (!commandArgs.TryAdd(paramName, value))
        throw new CommandParseException("Параметр указан дважды!");
    }

    return commandArgs;
  }

  private record CommandInfo(Func<IUserTaskService, IPresenter, IDictionary<string, string?>, ICommand> CommandCreator, string HelpSection);
}