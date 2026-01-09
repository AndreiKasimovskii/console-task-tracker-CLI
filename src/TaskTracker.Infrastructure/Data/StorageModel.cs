using System;

namespace TaskTracker.Infrastructure.Data;

public class StorageModel
{
    public long LastId { get; set; } = 0;

    public List<UserTaskStorageModel> Tasks { get; set; } = [];
}
