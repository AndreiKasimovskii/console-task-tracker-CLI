using TaskTracker.Domain.Data;

namespace TaskTracker.Application;

public static class OperationsErrorsHandler
{
  public static string ParseError(ErrorType? errorType)
  {
    return errorType switch
    {
      ErrorType.DuplicateTask => "Задача с таким загловком и сроком уже существует!",
      ErrorType.NotFound => "Задача не найдена",
      _ => "Неизвестный тип ошибки"
    };
  }
}
