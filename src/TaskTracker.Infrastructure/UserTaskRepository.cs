using System.Text.Json;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Exceptions;

namespace TaskTracker.Infrastructure;

public class UserTaskRepository(FileStore fileStore) : IUserTaskRepository
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    public async Task<UserTask> Create(UserTask task)
    {
        bool lockTaken = false;
        try
        {
            lockTaken = await SemaphoreSlim.WaitAsync(new TimeSpan(0, 0, 30));
            if(!lockTaken)
                throw new TimeoutException("Resource access timeout exceeded");

            var storageModel = await fileStore.ReadFromFileAsync();

            storageModel.Tasks.Add(new()
            {
                Id = ++storageModel.LastId,
                Title = task.Title,
                CreatedDate = task.CreatedDate,
                Status = task.Status.ToString(),
                Description = task.Description,
                Deadline = task.Deadline
            });

            await fileStore.WriteToFileAsync(storageModel);
            var createdTask = storageModel.Tasks.First(t => t.Id == storageModel.LastId);
            return UserTask.Restore(
                createdTask.Id, 
                createdTask.Title, 
                createdTask.Description, 
                createdTask.CreatedDate, 
                createdTask.Status,
                createdTask.Deadline);
        }
        catch(JsonException ex)
        {
            throw new InvalidDataException("Failed to save task", ex);
        }
        finally
        {
            if (lockTaken)
                SemaphoreSlim.Release();
        }
    }

    public async Task Delete(long id)
    {
        bool lockTaken = false;
        try
        {
            lockTaken = await SemaphoreSlim.WaitAsync(new TimeSpan(0, 0, 30));
            if(!lockTaken)
                throw new TimeoutException("Resource access timeout exceeded");

            var storageModel = await fileStore.ReadFromFileAsync();

            storageModel.Tasks.RemoveAll(t => t.Id == id);

            await fileStore.WriteToFileAsync(storageModel);
        }
        catch(JsonException ex)
        {
            throw new InvalidDataException("Failed to remove task", ex);
        }
        finally
        {
            if(lockTaken)
                SemaphoreSlim.Release();
        }
    }

    public async Task<UserTask[]> GetAllActive()
    {
        bool lockTaken = false;
        try
        {
            lockTaken = await SemaphoreSlim.WaitAsync(new TimeSpan(0, 0, 30));
            if(!lockTaken)
                throw new TimeoutException("Resource access timeout exceeded");

            var storageModel = await fileStore.ReadFromFileAsync();

            return [.. storageModel.Tasks
                .Where(t => t.Status == "Active")
                .Select(t => UserTask.Restore(t.Id, t.Title, t.Description, t.CreatedDate, t.Status, t.Deadline))];
        }
        catch(JsonException ex)
        {
            throw new InvalidDataException("Failed to read storage", ex);
        }
        catch(StorageCorruptedException ex)
        {
            throw new InvalidDataException("Data in the storage is corrupted", ex);
        }
        finally
        {
            if(lockTaken)
                SemaphoreSlim.Release();
        }
    }

    public async Task<UserTask?> GetById(long id)
    {
        bool lockTaken = false;
        try
        {
            lockTaken = await SemaphoreSlim.WaitAsync(new TimeSpan(0, 0, 30));
            if(!lockTaken)
                throw new TimeoutException("Resource access timeout exceeded");

            var storageModel = await fileStore.ReadFromFileAsync();

            var task = storageModel.Tasks.FirstOrDefault(t => t.Id == id);
            return task is not null 
                ? UserTask.Restore(task.Id, 
                    task.Title, 
                    task.Description,
                    task.CreatedDate, 
                    task.Status, 
                    task.Deadline)
                : null;
        }
        catch(JsonException ex)
        {
            throw new InvalidDataException("Failed to read storage", ex);
        }
        catch(StorageCorruptedException ex)
        {
            throw new InvalidDataException("Data in the storage is corrupted", ex);
        }
        finally
        {
            if(lockTaken)
                SemaphoreSlim.Release();
        }
    }

    public async Task<bool> HasDuplicate(string title, DateTimeOffset? deadline, long excludeId = -1)
    {
        bool lockTaken = false;
        try
        {
            lockTaken = await SemaphoreSlim.WaitAsync(new TimeSpan(0, 0, 30));
            if(!lockTaken)
                throw new TimeoutException("Resource access timeout exceeded");

            var storageModel = await fileStore.ReadFromFileAsync();
            var deadlineDate = deadline?.ToUniversalTime().Date;
            return storageModel.Tasks
                .Select(t => new { Task = t, DeadlineDate = t.Deadline?.ToUniversalTime().Date })
                .Any(t => t.Task.Status.Equals("Active", StringComparison.Ordinal)
                    && t.Task.Title.Equals(title, StringComparison.OrdinalIgnoreCase)
                    && deadlineDate == t.DeadlineDate
                    && t.Task.Id != excludeId);
        }
        catch(JsonException ex)
        {
            throw new InvalidDataException("Failed to read storage", ex);
        }
        finally
        {
            if(lockTaken)
                SemaphoreSlim.Release();
        }
    }

    public async Task<UserTask> Update(UserTask task)
    {
        bool lockTaken = false;
        try
        {
            lockTaken = await SemaphoreSlim.WaitAsync(new TimeSpan(0, 0, 30));
            if(!lockTaken)
                throw new TimeoutException("Resource access timeout exceeded");

            var storageModel = await fileStore.ReadFromFileAsync();

            var updatesTask = storageModel.Tasks.First(t => t.Id == task.Id);

            updatesTask.Title = task.Title;
            updatesTask.Status = task.Status.ToString();
            updatesTask.Description = task.Description;
            updatesTask.Deadline = task.Deadline;

            await fileStore.WriteToFileAsync(storageModel);
            return UserTask.Restore(
                updatesTask.Id,
                updatesTask.Title,
                updatesTask.Description,
                updatesTask.CreatedDate,
                updatesTask.Status,
                updatesTask.Deadline);
        }
        catch(JsonException ex)
        {
            throw new InvalidDataException("Failed to update task", ex);
        }
        finally
        {
            if(lockTaken)
                SemaphoreSlim.Release();
        }
    }
}
