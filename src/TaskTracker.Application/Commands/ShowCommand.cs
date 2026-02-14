using TaskTracker.Application.Commands.Common;
using TaskTracker.Domain.Abstractions;

namespace TaskTracker.Application.Commands;

internal class ShowCommand : CommandBase
{
    #region Parameters
    private readonly CommandParameter<long> _id = new("--id",
      (stringId) => long.TryParse(stringId, out var id)
        ? id
        : throw new CommandParseException($"Параметр {nameof(id)} представлен в неправильном формате")
    )
    {
        Required = true
    };
    #endregion

    public ShowCommand() : base("show")
    {
        Parameters.Add(_id);
    }

    public override async Task Execute(IUserTaskService userTaskService, IPresenter presenter)
    {
        var result = await userTaskService.ShowTask(_id.Value);

        if (result is { IsSuccess: true, UserTask: { } task })
        {
            presenter.PrintTask(task);
        }
        else
        {
            presenter.PrintWarning($"Удаление задачи с идентификатором {_id.Value} завершилось неуспешно");
            var errorMessage = OperationsErrorsHandler.ParseError(result.ErrorType);
            presenter.PrintError(errorMessage);
        }
    }
}
