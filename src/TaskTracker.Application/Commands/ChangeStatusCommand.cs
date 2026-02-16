using System;
using TaskTracker.Application.Commands.Common;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Commands;

internal class ChangeStatusCommand : CommandBase
{
    #region Parameters
    private readonly CommandParameter<long> _id = new("--id", (v) =>
        !long.TryParse(v, out var id)
        ? throw new CommandParseException($"Параметр {nameof(id)} представлен в неправильном формате")
        : id)
    {
        Required = true
    };

    private readonly CommandParameter<UserTaskStatus> _toStatus = new("--to", (v) => 
        !Enum.TryParse<UserTaskStatus>(v, out var toStatus)
        ? throw new CommandParseException($"Параметр {nameof(toStatus)} представлен в неправильном формате")
        : toStatus)
    {
        Required = true
    };
    #endregion

    public ChangeStatusCommand() : base("change-status")
    {
        Parameters.Add(_id);
        Parameters.Add(_toStatus);
    }

    public override async Task Execute(IUserTaskService userTaskService, IPresenter presenter)
    {
        var operationResult = await userTaskService.ChangeStatus(_id.Value, _toStatus.Value);

        if (operationResult.IsSuccess)
        {
            presenter.Print($"Для задачи с идентификатором {_id.Value} статус изменен на {_toStatus.Value}");
        }
        else
        {
            presenter.PrintWarning($"Изменение статуса задачи {_id.Value} завершилось неуспешно");
            var errorMessage = OperationsErrorsHandler.ParseError(operationResult.ErrorType);
            presenter.PrintError(errorMessage);
        }
    }
}
