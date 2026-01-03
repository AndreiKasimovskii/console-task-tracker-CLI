using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Abstractions;

public interface IUserTaskRepository
{
    Task<UserTask?> GetById(long id);

    Task Create(UserTask task);

    Task Update(UserTask task);

    Task Delete(long id);

    Task<UserTask[]> GetAllActive();

    Task<bool> HasDuplicate(string title, DateTimeOffset? deadline, long excludeId = -1);
}
