using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Data;

public record OperationResult
{
    public bool IsSuccess { get; init; }

    public ErrorType? ErrorType { get; init; }

    public UserTask? UserTask { get; init; }

    private OperationResult(bool isSuccess, ErrorType? errorType = null,
        UserTask? userTask = null)
    {
        IsSuccess = isSuccess;
        ErrorType = errorType;
        UserTask = userTask;
    }

    public static OperationResult Success()
    {
        return new OperationResult(true);
    }

    public static OperationResult Success(UserTask userTask)
    {
        return new OperationResult(true, userTask: userTask);
    }

    public static OperationResult Failure(ErrorType errorType)
    {
        return new OperationResult(false, errorType);
    }
}
