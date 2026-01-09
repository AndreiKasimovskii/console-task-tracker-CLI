using System.IO.Compression;
using Microsoft.VisualBasic;

namespace TaskTracker.Domain.Entities;

public class UserTask(string title)
{
    public long Id { get; init; }

    public string Title { get; private set; } = title;

    public string? Description { get; set; }

    public DateTimeOffset CreatedDate { get; init; } = DateTimeOffset.UtcNow;

    public UserTaskStatus Status { get; private set; } = UserTaskStatus.Active;

    public DateTimeOffset? Deadline { get; private set; }

    private UserTask(long id, string title, DateTimeOffset createdDate, UserTaskStatus status, string? description, DateTimeOffset? deadline)
        : this(title)
    {
        Id = id;
        CreatedDate = createdDate;
        Status = status;
        Description = description;
        Deadline = deadline;
    }

    #region Methods
    public bool TryChangeStatus(UserTaskStatus newStatus)
    {
        if(Status is UserTaskStatus.Completed or UserTaskStatus.Cancelled)
            return false;

        if(Status == UserTaskStatus.Active && newStatus is UserTaskStatus.Active)
            return false;

        Status = newStatus;
        return true;
    }

    public bool TryChangeTitle(string? newTitle)
    {
        if(string.IsNullOrWhiteSpace(newTitle))
            return false;

        Title = newTitle.Trim();
        return true;
    }

    public bool TrySetDeadline(DateTimeOffset? deadline)
    {
        if(deadline.HasValue && 
            CreatedDate.CompareTo(deadline.Value) > 0)
            return false;

        Deadline = deadline;
        return true;
    }

    internal static UserTask Restore(long id, string title, string? description,
        DateTimeOffset createdDate, string? status, DateTimeOffset? deadline)
    {
        var validationResult = ValidateRestoredParameters(id, title, createdDate, status, deadline);
        if(validationResult.Success)
            return new(id, title, createdDate, validationResult.ParsedTaskStatus, description, deadline);
        else
            throw new StorageCorruptedException(validationResult.FailedMessage);
    }

    private static ValidationRestoredParametersResult ValidateRestoredParameters(long id, string title, DateTimeOffset createdDate, string? status, DateTimeOffset? deadline)
    {
        if(id < 0)
            return new(false, "Error! Id lower then 0.");

        if(string.IsNullOrWhiteSpace(title))
            return new(false, "Error! Title is empty.");

        if(createdDate.Equals(default))
            return new(false, "Error! Creation date not set.");

        if(!Enum.TryParse<UserTaskStatus>(status, out var statusValue))
            return new(false, "Error! Task status has unknown value.");

        if(deadline.HasValue && deadline.Value.CompareTo(createdDate) < 0)
            return new(false, "Error! Task deadline cannot be earlier than the creation date");

        return new(true, ParsedTaskStatus: statusValue);
    }

    private record ValidationRestoredParametersResult(bool Success, string? FailedMessage = null, UserTaskStatus ParsedTaskStatus = default);
    #endregion
}

public class StorageCorruptedException(string? message) : Exception(message) { }