using System.Collections.ObjectModel;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Data;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Services;

public class UserTaskService(IUserTaskRepository taskRepository) : IUserTaskService
{
    public async Task<OperationResult> AddTask(UserTask task)
    {
        try
        {
            if(task.Deadline.HasValue
                && task.Deadline.Value.Date.CompareTo(task.CreatedDate.Date) < 0)
            {
                return OperationResult.Failure($"Task deadline ({task.Deadline.Value.Date}) cannot be earlier"
                    + $" than the date of the task({task.CreatedDate})", ErrorType.ValidationError);
            }
            
            var similarTask = await taskRepository.Find(t => t.Title == task.Title);
            if(similarTask is not null)
            {
                if(similarTask.Status == UserTaskStatus.Active && similarTask.Deadline.Equals(task.Deadline))
                    return OperationResult.Failure($"Similiar task already exists (Id: {similarTask.Id})",
                        ErrorType.ValidationError);
            }

            await taskRepository.Create(task);
            return OperationResult.Success();
        }
        catch(Exception ex)
        {
            return OperationResult.Failure($"{ex.GetType().Name}: {ex.Message}", ErrorType.UnknownException);
        }
    }

    public async Task<OperationResult> EditTask(long taskId, Parameter<string> title, Parameter<string> description,
        Parameter<DateTimeOffset?> deadline)
    {
        try
        {
            var editedTask = await taskRepository.GetById(taskId);
            if(editedTask is null)
                return OperationResult.Failure($"Task with identifier = {taskId} does not exists.",
                    ErrorType.NotFound);
        
            if(!deadline.NotChange && deadline.Value.HasValue
                && deadline.Value.Value.Date.CompareTo(editedTask.CreatedDate.Date) < 0)
            {
                return OperationResult.Failure($"Task deadline ({deadline.Value.Value.Date}) cannot be earlier"
                + $" than the date of the task({editedTask.CreatedDate})", ErrorType.ValidationError);
            }

            if(!title.NotChange && title.Value is not null)
                editedTask.Title = title.Value;

            if(!description.NotChange)
                editedTask.Description = description.Value;

            if(!deadline.NotChange)
                editedTask.Deadline = deadline.Value;

            await taskRepository.Update(editedTask);
            return OperationResult.Success();
        }
        catch(Exception ex)
        {
            return OperationResult.Failure($"{ex.GetType().Name}: {ex.Message}", ErrorType.UnknownException);
        }
    }

    public async Task<OperationResult> RemoveTask(long taskId)
    {
        try
        {
            var deletedTask = taskRepository.GetById(taskId);
            if(deletedTask is null)
                return OperationResult.Failure($"Task with identifier = {taskId} does not exists.",
                    ErrorType.NotFound);

            await taskRepository.Delete(taskId);
            return OperationResult.Success();
        }
        catch(Exception ex)
        {
            return OperationResult.Failure($"{ex.GetType().Name}: {ex.Message}", ErrorType.UnknownException);
        } 
    }

    public async Task<OperationResult> ShowTask(long taskId)
    {
        try
        {
            var task = await taskRepository.GetById(taskId);
            if (task is null)
                return OperationResult.Failure($"Task with identifier = {taskId} does not exists.",
                    ErrorType.NotFound);

            return OperationResult.Success(task);
        }
        catch(Exception ex)
        {
            return OperationResult.Failure($"{ex.GetType().Name}: {ex.Message}", ErrorType.UnknownException);
        }
    }

    public async Task<OperationResult> ChangeStatus(long taskId, UserTaskStatus newTaskStatus)
    {
        try
        {
            var task = await taskRepository.GetById(taskId);
            if (task is null)
                return OperationResult.Failure($"Task with identifier = {taskId} does not exists.",
                    ErrorType.NotFound);

            if(!CanChangeStatus(task.Status, newTaskStatus))
            {
                return OperationResult.Failure("Cannot change status", ErrorType.NotFound);
            }

            task.Status = newTaskStatus;
            await taskRepository.Update(task);
            return OperationResult.Success();
        }
        catch(Exception ex)
        {
            return OperationResult.Failure($"{ex.GetType().Name}: {ex.Message}", ErrorType.UnknownException);
        }
    }

    public async Task<ReadOnlyCollection<UserTask>> ShowTasksList()
    {
        return new ReadOnlyCollection<UserTask>(await taskRepository.GetAllActive());
    }

    private bool CanChangeStatus(UserTaskStatus currentTaskStatus, UserTaskStatus newTaskStatus)
    {
        return currentTaskStatus == UserTaskStatus.Active;
    }
}