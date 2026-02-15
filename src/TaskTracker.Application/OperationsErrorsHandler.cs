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
      ErrorType.InputDateLaterDeadline => "Срок задачи указан некорректно! Он не может быть раньше даты создания задачи или текущей даты",
      ErrorType.InvalidTitle => "Заголовок задачи не может быть пустым или состоять только из пробельных символов",
      _ => "Неизвестный тип ошибки"
    };
  }
}
