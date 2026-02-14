using TaskTracker.Domain.Entities;

namespace TaskTracker.Application;

interface IPresenter
{
  void PrintError(string error);

  void PrintWarning(string warning);

  void PrintHint(string message);

  void Print(string message);

  void PrintTasks(IEnumerable<UserTask> userTasks);

  void PrintTask(UserTask task);

  void PrintQuestion(string question);
}