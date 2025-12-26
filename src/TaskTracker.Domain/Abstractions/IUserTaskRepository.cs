using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Abstractions;

public interface IUserTaskRepository
{
    Task<UserTask?> GetById(long id);

    Task Create(UserTask task);

    Task Update(UserTask task);

    Task<bool> IsExists(long id);

    Task Delete(long id);

    Task<UserTask[]> GetAllActive();

    Task<UserTask?> Find(Func<UserTask, bool> predicate);
}
