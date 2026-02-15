namespace TaskTracker.Application;

class CommandParameter<T>(string name, Func<string, T> parse) : ICommandParameter
{
    public string Name { get; init; } = name;

    public T? Value { get; private set; }

    private readonly bool _required;
    public bool Required
    {
        get => _required;
        init
        {
            if (value)
                NotDefined = false;
            _required = value;
        }
    }

    public bool IsFlag { get; init; } = false;

    public bool NotDefined { get; set; } = true;

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

    bool NotDefined { get; set; }

    void SetValue(string value);
}