using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Data;

public class OperationResult
{
    public bool IsSuccess { get; }

    public string? ErrorMessage { get; }

    public ErrorType? ErrorType { get; set; }

    public UserTask? UserTask { get; set; }

    private OperationResult(bool isSuccess, string? errorMessage = null, ErrorType? errorType = null,
        UserTask? userTask = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
        UserTask = userTask;
    }

    public static OperationResult Success()
    {
        return new OperationResult(true);
    }

    public static OperationResult Failure(string errorMessage, ErrorType errorType)
    {
        return new OperationResult(false, errorMessage, errorType);
    }

    public static OperationResult Success(UserTask userTask)
    {
        return new OperationResult(true, userTask: userTask);
    }
}
