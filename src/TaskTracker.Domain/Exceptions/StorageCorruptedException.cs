namespace TaskTracker.Domain.Exceptions;

public class StorageCorruptedException(string? message) : Exception(message);