using System.Collections.ObjectModel;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Data;
using TaskTracker.Domain.DataTransferObjects;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Services;

public class UserTaskService(IUserTaskRepository taskRepository) : IUserTaskService
{
    public async Task<OperationResult> AddTask(UserTaskDto task)
    {
        UserTask newTask = new(task.Title)
        {
            Description = task.Description,
        };

        if(!newTask.TrySetDeadline(task.Deadline))
            return OperationResult.Failure(ErrorType.InputDateLaterDeadline);

        if(await taskRepository.HasDuplicate(newTask.Title, newTask.Deadline))
            return OperationResult.Failure(ErrorType.DuplicateTask);

        var createdTask = await taskRepository.Create(newTask);
        return OperationResult.Success(createdTask);
    }

    public async Task<OperationResult> EditTask(long taskId, Parameter<string> title, Parameter<string> description,
        Parameter<DateTimeOffset?> deadline)
    {
        var editedTask = await taskRepository.GetById(taskId);
        if(editedTask is null)
            return OperationResult.Failure(ErrorType.NotFound);

        if(!title.NotChange)
            if(!editedTask.TryChangeTitle(title.Value))
                return OperationResult.Failure(ErrorType.InvalidTitle);

        if(!description.NotChange)
            editedTask.Description = description.Value;

        if(!deadline.NotChange)
            if(!editedTask.TrySetDeadline(deadline.Value))
                return OperationResult.Failure(ErrorType.InputDateLaterDeadline);

        if(await taskRepository.HasDuplicate(editedTask.Title, editedTask.Deadline, editedTask.Id))
            return OperationResult.Failure(ErrorType.DuplicateTask);

        var updatedTask = await taskRepository.Update(editedTask);
        return OperationResult.Success(updatedTask);
    }

    public async Task<OperationResult> RemoveTask(long taskId)
    {
        var deletedTask = await taskRepository.GetById(taskId);
        if(deletedTask is null)
            return OperationResult.Failure(ErrorType.NotFound);

        await taskRepository.Delete(taskId);
        return OperationResult.Success();
    }

    public async Task<OperationResult> ShowTask(long taskId)
    {
        var task = await taskRepository.GetById(taskId);
        return task is null 
            ? OperationResult.Failure(ErrorType.NotFound) 
            : OperationResult.Success(task);
    }

    public async Task<OperationResult> ChangeStatus(long taskId, UserTaskStatus newTaskStatus)
    {
        var task = await taskRepository.GetById(taskId);
        if (task is null)
            return OperationResult.Failure(ErrorType.NotFound);

        if(!task.TryChangeStatus(newTaskStatus))
        {
            return OperationResult.Failure(ErrorType.IncorrectStatus);
        }
        
        await taskRepository.Update(task);
        return OperationResult.Success();
    }

    public async Task<ReadOnlyCollection<UserTask>> ShowTasksList()
    {
        return new ReadOnlyCollection<UserTask>(await taskRepository.GetAllActive());
    }
}