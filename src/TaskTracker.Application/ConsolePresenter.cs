using TaskTracker.Domain.Entities;

namespace TaskTracker.Application;

internal class ConsolePresenter : IPresenter
{
  public void PrintError(string error)
  {
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(error);
    Console.ResetColor();
  }

  public void PrintHint(string message)
  {
    Console.WriteLine("\n\n");
    Console.WriteLine(message);
    Console.WriteLine("\n\n");
  }

  public void Print(string message)
  {
    Console.WriteLine(message);
  }

  public void PrintTasks(IEnumerable<UserTask> userTasks)
  {
    foreach (var task in userTasks)
    {
      Console.WriteLine($"[#{task.Id}] {task.Title} {(task.Deadline.HasValue ? task.Deadline.Value.ToString("yyyy-MM-dd") : "none")}");
    }
  }

    public void PrintQuestion(string question)
    {
      Console.Write(question + " ");
    }

    public void PrintWarning(string warning)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine(warning);
      Console.ResetColor();
    }

    public void PrintTask(UserTask task)
    {
      Console.WriteLine($"Task [#{task.Id}] | {task.CreatedDate}\n" +
                        $"[{task.Status}]\n" +
                        $"{task.Title} \n" +
                        $"Описание:\n\t{task.Description}\n" +
                        $"Срок: {(task.Deadline.HasValue ? task.Deadline.Value.ToString("yyyy-MM-dd") : "none")}");
    }
}