namespace TaskTracker.Infrastructure.Data;

public class UserTaskStorageModel
{
    public long Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public required string Status { get; set; }

    public DateTimeOffset? Deadline { get; set; }
}
