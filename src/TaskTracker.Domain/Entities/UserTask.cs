namespace TaskTracker.Domain.Entities;

public class UserTask
{
    public long Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public UserTaskStatus Status { get; set; }

    public DateTimeOffset? Deadline { get; set; }
}
