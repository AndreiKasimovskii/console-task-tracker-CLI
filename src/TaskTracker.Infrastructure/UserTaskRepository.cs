using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure;

public class UserTaskRepository(FileStore fileStore) : IUserTaskRepository
{
    private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public async Task Create(UserTask task)
    {
        await _semaphoreSlim.WaitAsync();

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

        _semaphoreSlim.Release();
    }

    public async Task Delete(long id)
    {
        await _semaphoreSlim.WaitAsync();

        var storageModel = await fileStore.ReadFromFileAsync();

        storageModel.Tasks.RemoveAll(t => t.Id == id);

        await fileStore.WriteToFileAsync(storageModel);

        _semaphoreSlim.Release();
    }

    public async Task<UserTask[]> GetAllActive()
    {
        await _semaphoreSlim.WaitAsync();

        var storageModel = await fileStore.ReadFromFileAsync();

        _semaphoreSlim.Release();

        return [.. storageModel.Tasks.Select(t => UserTask.Restore(t.Id, t.Title, t.Description, t.CreatedDate, t.Status, t.Deadline))];
    }

    public async Task<UserTask?> GetById(long id)
    {
        await _semaphoreSlim.WaitAsync();

        var storageModel = await fileStore.ReadFromFileAsync();

        _semaphoreSlim.Release();

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

    public async Task<bool> HasDuplicate(string title, DateTimeOffset? deadline, long excludeId = -1)
    {
        await _semaphoreSlim.WaitAsync();

        var storageModel = await fileStore.ReadFromFileAsync();

        _semaphoreSlim.Release();

        var task = storageModel.Tasks.FirstOrDefault(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase)
            && t.Deadline.Equals(deadline?.ToUniversalTime())
            && t.Id != excludeId);
        return task is not null;
    }

    public async Task Update(UserTask task)
    {
        await _semaphoreSlim.WaitAsync();

        var storageModel = await fileStore.ReadFromFileAsync();

        var updatesTask = storageModel.Tasks.First(t => t.Id == task.Id);

        updatesTask.Title = task.Title;
        updatesTask.Status = task.Status.ToString();
        updatesTask.Description = task.Description;
        updatesTask.Deadline = task.Deadline;

        await fileStore.WriteToFileAsync(storageModel);

        _semaphoreSlim.Release();
    }
}
