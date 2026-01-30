namespace TaskTracker.Application;

class CommandParameter<T>(string name, Func<string, T> parse) : ICommandParameter
{
    public string Name { get; init; } = name;

    public T? Value { get; private set; }

    public bool Required { get; init; } = false;

    public bool IsFlag { get; init; } = false;

    public void SetValue(string value)
    {
        Value = parse(value);
    }
}

interface ICommandParameter
{
    string Name { get; }

    bool Required { get; }

    bool IsFlag { get; }

    void SetValue(string value);
}