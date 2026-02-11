using TaskTracker.Domain.Entities;

namespace TaskTracker.Application;

class ConsolePresenter : IPresenter
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

    public void PrintTask(UserTask? task)
    {
      if (task is null)
      {
        Console.WriteLine("No task data...");
        return;
      }

      Console.WriteLine($"[#{task.Id}] \n" +
                        $"{task.Title} \n" +
                        $"Статус:\n{task.Status}\n" +
                        $"Описание:\n{task.Description}\n" +
                        $"Создана:\n{task.CreatedDate}\n" +
                        $"Срок: {(task.Deadline.HasValue ? task.Deadline.Value.ToString("yyyy-MM-dd") : "none")}");
    }
}