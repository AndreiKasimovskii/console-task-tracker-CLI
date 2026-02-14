using TaskTracker.Domain.Abstractions;

namespace TaskTracker.Application;

interface ICommand
{
  Task Execute(IUserTaskService userTaskService, IPresenter presenter);
}