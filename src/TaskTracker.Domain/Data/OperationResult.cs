using System;

namespace TaskTracker.Domain.Data;

public class OperationResult
{
    public bool IsSuccess { get; }

    public string? ErrorMessage { get; }

    public ErrorType? ErrorType { get; set; }

    private OperationResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static OperationResult Success()
    {
        return new OperationResult(true);
    }

    public static OperationResult Failure(string errorMessage)
    {
        return new OperationResult(false, errorMessage);
    }
}
