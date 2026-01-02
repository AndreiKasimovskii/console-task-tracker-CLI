using System.Collections.ObjectModel;

namespace TaskTracker.Domain.Services;

public class UserTaskService(IUserTaskRepository taskRepository) : IUserTaskService
{
    public async Task<OperationResult> AddTask(UserTask task)
    {
        if(task.Deadline.HasValue
            && task.Deadline.Value.Date.CompareTo(task.CreatedDate.Date) < 0)
        {
            return OperationResult.Failure(ErrorType.InputDateLaterDeadline);
        }
            
        var similarTask = await taskRepository.Find(t => t.Title == task.Title);
        if(similarTask is not null)
        {
            if(similarTask.Status == UserTaskStatus.Active && similarTask.Deadline.Equals(task.Deadline))
                return OperationResult.Failure(ErrorType.DuplicateTask);
        }

        await taskRepository.Create(task);
        return OperationResult.Success();
    }

    public async Task<OperationResult> EditTask(long taskId, Parameter<string> title, Parameter<string> description,
        Parameter<DateTimeOffset?> deadline)
    {
        var editedTask = await taskRepository.GetById(taskId);
        if(editedTask is null)
            return OperationResult.Failure(ErrorType.NotFound);
        
        if(!deadline.NotChange && deadline.Value.HasValue
            && deadline.Value.Value.Date.CompareTo(editedTask.CreatedDate.Date) < 0)
        {
            return OperationResult.Failure(ErrorType.InputDateLaterDeadline);
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
        if (task is null)
            return OperationResult.Failure(ErrorType.NotFound);

        return OperationResult.Success(task);
    }

    public async Task<OperationResult> ChangeStatus(long taskId, UserTaskStatus newTaskStatus)
    {
        var task = await taskRepository.GetById(taskId);
        if (task is null)
            return OperationResult.Failure(ErrorType.NotFound);

        if(!CanChangeStatus(task.Status))
        {
            return OperationResult.Failure(ErrorType.IncorrectStatus);
        }

        task.Status = newTaskStatus;
        await taskRepository.Update(task);
        return OperationResult.Success();
    }

    public async Task<ReadOnlyCollection<UserTask>> ShowTasksList()
    {
        return new ReadOnlyCollection<UserTask>(await taskRepository.GetAllActive());
    }

    private bool CanChangeStatus(UserTaskStatus currentTaskStatus)
    {
        return currentTaskStatus == UserTaskStatus.Active;
    }
}