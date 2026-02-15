using TaskTracker.Application.Commands.Common;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Data;

namespace TaskTracker.Application.Commands;

internal class EditCommand : CommandBase
{
  #region Parameters
  private readonly CommandParameter<long> _id = new("--id", (v)
    => !long.TryParse(v, out var id)
      ? throw new CommandParseException($"Не правильно указан формат параметра {nameof(id)}")
      : id)
  {
    Required = true
  };
  
  private readonly CommandParameter<string> _title = new("--title", v => v);
  private readonly CommandParameter<string?> _description = new("--desc", v => v);
  private readonly CommandParameter<DateTimeOffset?> _deadline = new("--deadline", v
      => !DateTimeOffset.TryParse(v, out var deadline)
      ? throw new CommandParseException($"Не правильно указан формат параметра {nameof(deadline)}")
      : deadline);
  #endregion

  public EditCommand() : base("edit")
  {
    Parameters.Add(_id);
    Parameters.Add(_title);
    Parameters.Add(_description);
    Parameters.Add(_deadline);
  }

  public override async Task Execute(IUserTaskService userTaskService, IPresenter presenter)
  {
    if (!Parameters.Any(p => p is { Required: false, NotDefined: false }))
    {
      presenter.PrintWarning("Операция редактирования не будет выполнена. Нет изменений параметров задачи.");
      return;
    }

    var operationResult = await userTaskService.EditTask(
      _id.Value,
      new Parameter<string>(_title.NotDefined, _title.Value),
      new Parameter<string>(_description.NotDefined, _description.Value),
      new Parameter<DateTimeOffset?>(_deadline.NotDefined, _deadline.Value));

    if (operationResult is { IsSuccess: true, UserTask: { } task })
    {
      presenter.Print("Задача успешно изменена!");
      presenter.PrintTask(task);
    }
    else
    {
      var errorMessage = OperationsErrorsHandler.ParseError(operationResult.ErrorType);
      presenter.PrintError(errorMessage);
    }
  }
}
