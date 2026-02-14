using TaskTracker.Application.Commands.Common;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.DataTransferObjects;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Commands;

internal class AddCommand : CommandBase, ICommand
{
    #region Parameters
    private readonly CommandParameter<string> _title = new("--title", v => v)
    {
        Required = true
    };
    private readonly CommandParameter<string?> _description = new("--desc", v => v);
    private readonly CommandParameter<DateTimeOffset?> _deadline = new("--deadline", v 
        => !DateTimeOffset.TryParse(v, out var deadline) 
        ? throw new CommandParseException($"Не правильно указан формат параметра {nameof(deadline)}") 
        : deadline);
    
    //private ICommandParameter[] Parameters { get; }
    #endregion

    public AddCommand() : base("add")
    {
        Parameters.Add(_title);
        Parameters.Add(_description);
        Parameters.Add(_deadline);
    }
    
    public override async Task Execute(IUserTaskService userTaskService, IPresenter presenter)
    {
        UserTaskDto dto = new(0, 
            _title.Value!, 
            _description.Value, 
            DateTimeOffset.Now, 
            UserTaskStatus.Active, 
            _deadline.Value);

        var operationResult = await userTaskService.AddTask(dto);

        if (operationResult is {IsSuccess:true, UserTask: {} task})
        {
            presenter.Print("Задача успешно создана!");
            presenter.PrintTask(task);
        }
        else
        {
            presenter.PrintWarning("Задача не была создана.");
            var errorMessage = OperationsErrorsHandler.ParseError(operationResult.ErrorType);
            presenter.PrintError(errorMessage);
        }
    }
}
