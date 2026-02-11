using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.DataTransferObjects;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application;

class AddCommand : ICommand
{
    private readonly IUserTaskService _userTaskService;
    private readonly IPresenter _presenter;

    private readonly CommandParameter<string> _title = new("--title", v => v)
    {
        Required = true
    };
    private readonly CommandParameter<string?> _description = new("--desc", v => v);
    private readonly CommandParameter<DateTimeOffset?> _deadline = new("--deadline", v 
        => !DateTimeOffset.TryParse(v, out var deadline) 
        ? throw new CommandParseException($"Не правильно указан формат параметра {nameof(deadline)}") 
        : deadline);

    private readonly List<ICommandParameter> _parameters;

    private AddCommand(IUserTaskService userTaskService, IPresenter presenter)
    {
        _userTaskService = userTaskService;
        _presenter = presenter;
        _parameters = [_title, _description, _deadline];
    }
    
    public async Task Execute()
    {
        UserTaskDto dto = new(0, 
            _title.Value!, 
            _description.Value, 
            DateTimeOffset.Now, 
            UserTaskStatus.Active, 
            _deadline.Value);

        var operationResult = await _userTaskService.AddTask(dto);

        if (operationResult.IsSuccess)
        {
            _presenter.Print("Задача успешно создана!");
            _presenter.PrintTask(operationResult.UserTask);
        }
        else
        {
            _presenter.PrintWarning("Задача не была создана:");
            var errorMessage = OperationsErrorsHandler.ParseError(operationResult.ErrorType);
            _presenter.PrintError(errorMessage);
        }
    }

    public static ICommand CreateCommand(IUserTaskService userTaskService, IPresenter presenter,
        IDictionary<string, string?> parameters)
    {
        var command = new AddCommand(userTaskService, presenter);
        command.ParseParameters(parameters);
        return command;
    }

    private void ParseParameters(IDictionary<string, string?> args)
    {
        if (args.Keys.Except(_parameters.Select(p => p.Name)).Any())
            throw new CommandParseException("Указаны неизвестные параметры для команды.");
        var requiredParameters = _parameters
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

        var optionalParameters = _parameters.Except(requiredParameters);
        foreach (var optionalParameter in optionalParameters)
        {
            if (!args.TryGetValue(optionalParameter.Name, out var value)) continue;
            
            if(optionalParameter.IsFlag)
                optionalParameter.SetValue("true");
            else
            {
                if(value is null)
                    throw new CommandParseException(
                        $"Не указано значение для параметра {optionalParameter.Name}");
                optionalParameter.SetValue(value);
            }
        }
    }
}
