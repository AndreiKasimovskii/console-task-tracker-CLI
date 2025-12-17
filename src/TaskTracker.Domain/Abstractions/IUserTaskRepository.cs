using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Abstractions;

public interface IUserTaskRepository
{
    Task<UserTask?> GetTask(string title, DateTimeOffset? deadline);

    Task<UserTask?> GetTask(long id);

    Task CreateTask(UserTask task);

    Task UpdateTask(UserTask task);

    Task<bool> IsExists(long id);

    Task DeleteTask(long id);

    Task<UserTask[]> GetActiveTasks();
}
