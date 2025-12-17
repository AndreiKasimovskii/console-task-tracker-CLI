namespace TaskTracker.Domain.Data;

public record Parameter<T>(bool NotChange, T? Value);
