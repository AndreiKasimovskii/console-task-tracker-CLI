using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.DataTransferObjects;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Title"></param>
/// <param name="Description"></param>
/// <param name="CreatedDate"></param>
/// <param name="Status"></param>
/// <param name="Deadline"></param>
public record UserTaskDto(
    long Id, 
    string Title, 
    string? Description, 
    DateTimeOffset CreatedDate, 
    UserTaskStatus Status, 
    DateTimeOffset? Deadline);
