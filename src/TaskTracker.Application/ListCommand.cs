using TaskTracker.Domain.Abstractions;

namespace TaskTracker.Application;

class ListCommand : CommandBase
{
  public ListCommand() : base("list")
  {
    Parameters = [];
  }

  public override async Task Execute(IUserTaskService userTaskService, IPresenter presenter)
  {
    var tasks = await userTaskService.ShowTasksList();

    if (tasks.Count == 0)
    {
      presenter.Print("Нет активных задач!");
      return;
    }
        
    var orderedTasks = tasks
      .OrderBy(t => t.Deadline, new DeadlineComparer())
      .ThenBy(t => t.CreatedDate);
    presenter.PrintTasks(orderedTasks);
  }
}
