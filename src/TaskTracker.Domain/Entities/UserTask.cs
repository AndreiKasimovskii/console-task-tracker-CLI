namespace TaskTracker.Domain.Entities;

public class UserTask(string title)
{
    public long Id { get; set; }

    public string Title { get; private set; } = title;

    public string? Description { get; set; }

    public DateTimeOffset CreatedDate { get; init; } = DateTimeOffset.UtcNow;

    public UserTaskStatus Status { get; private set; } = UserTaskStatus.Active;

    public DateTimeOffset? Deadline { get; private set; }

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
    #endregion
}
