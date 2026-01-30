using TaskTracker.Domain.Abstractions;

namespace TaskTracker.Application;

class ListCommand : ICommand
{
  private readonly IUserTaskService _userTaskService;
  private readonly IPresenter _presenter;

  private ListCommand(IUserTaskService userTaskService, IPresenter presenter)
  {
    _userTaskService = userTaskService;
    _presenter = presenter;
  }

  public static ICommand CreateCommand(IUserTaskService service, IPresenter presenter, IDictionary<string,string?> _)
  {
    return new ListCommand(service, presenter);
  }

  public async Task Execute()
  {
    var tasks = await _userTaskService.ShowTasksList();

    if (tasks.Count == 0)
    {
      _presenter.Print("Нет активных задач!");
      return;
    }
        
    var orderedTasks = tasks
      .OrderBy(t => t.Deadline, new DeadlineComparer())
      .ThenBy(t => t.CreatedDate);
    _presenter.PrintTasks(orderedTasks);
  }
}

class DeadlineComparer : IComparer<DateTimeOffset?>
{
    public int Compare(DateTimeOffset? x, DateTimeOffset? y)
    {
      if (x is null && y is null)
        return 0;

      if (x is null && y is not null)
        return 1;

      if (x is not null && y is null)
        return -1;

      return ((DateTimeOffset)x!).CompareTo((DateTimeOffset)y!);
    }
}