using TaskTracker.Domain.Abstractions;

namespace TaskTracker.Application.Commands.Common;

interface ICommand
{
  Task Execute(IUserTaskService userTaskService, IPresenter presenter);
}