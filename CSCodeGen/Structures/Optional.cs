namespace CSCodeGen.Structures;

public class Optional<T>
{
    private T? _value;

    public Optional()
    {
    }

    public Optional(T? value)
    {
        Value = value;
    }

    public T? Value
    {
        get => _value;
        set
        {
            _value = value;
            HasValue = true;
        }
    }

    public void ClearValue()
    {
        HasValue = false;
    }

    public bool HasValue { get; private set; }
}

public class Optional : Optional<object>
{
    public Optional()
    {
    }

    public Optional(object? value) : base(value)
    {
    }
}