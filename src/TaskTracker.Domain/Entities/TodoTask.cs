namespace TaskTracker.Domain.Entities;

public class TodoTask
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public TodoTaskStatus Status { get; set; }

    public DateTimeOffset? DeadLineDate { get; set; }

    public DateTimeOffset? CompleteDate { get; set; }
}
