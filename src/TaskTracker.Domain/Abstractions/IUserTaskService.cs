using System.Collections.ObjectModel;
using TaskTracker.Domain.Data;
using TaskTracker.Domain.DataTransferObjects;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Abstractions;

public interface IUserTaskService
{
    Task<ReadOnlyCollection<UserTask>> ShowTasksList();

    Task<OperationResult> ChangeStatus(long taskId, UserTaskStatus newTaskStatus);

    Task<OperationResult> ShowTask(long taskId);

    Task<OperationResult> RemoveTask(long taskId);

    Task<OperationResult> EditTask(long taskId, Parameter<string> title, Parameter<string> description,
        Parameter<DateTimeOffset?> deadline);

    Task<OperationResult> AddTask(UserTaskDto task);
}