namespace TaskTracker.Domain.Data;

public enum ErrorType
{
    NotFound,
    InputDateLaterDeadline,
    DuplicateTask,
    IncorrectStatus,
    InvalidTitle,
    UnknownException
}
