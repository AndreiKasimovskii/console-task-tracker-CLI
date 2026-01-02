using System.Collections.ObjectModel;

namespace TaskTracker.Domain.Services;

public interface IUserTaskService
{
    Task<ReadOnlyCollection<UserTask>> ShowTasksList();

    Task<OperationResult> ChangeStatus(long taskId, UserTaskStatus newTaskStatus);

    Task<OperationResult> ShowTask(long taskId);

    Task<OperationResult> RemoveTask(long taskId);

    Task<OperationResult> EditTask(long taskId, Parameter<string> title, Parameter<string> description,
        Parameter<DateTimeOffset?> deadline);

    Task<OperationResult> AddTask(UserTask task);
}